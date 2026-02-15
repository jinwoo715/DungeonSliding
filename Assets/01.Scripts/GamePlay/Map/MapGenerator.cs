using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MapGenerator : MonoBehaviour
    {
        // ---------- Public Config ----------
        public struct Config
        {
            public int Width;
            public int Height;

            public int EnemyCount;
            public int StopCount;
            public int TurnLeftCount;
            public int TurnRightCount;
            public int TeleportPairCount;

            // "벽 비율"로 쓰되, 이 버전은 "목표 Road 수"를 정하는 용도로 사용
            public float WallFill;              // 0~0.6 권장

            public int MaxAttempts;
            public int MaxPlacementTries;
            public int Seed;

            public string EnemyUidPrefix;

            // Carving params
            public int CarveMaxSteps;           // 복도 길이 최대
            public int CarveTargetRoadRatioX100;// 내부 road 비율(%) 목표 (WallFill 기반 자동계산 대신 강제하고 싶을 때)

            // Validation
            public bool RequireNoDeadRoad;      // "지나갈 수도/멈출 수도 없는 road" 0개 강제

            public static Config Default(int w, int h, int seed, float wallFill)
            {
                return new Config
                {
                    Width = w,
                    Height = h,

                    EnemyCount = 2,
                    StopCount = 0,
                    TurnLeftCount = 0,
                    TurnRightCount = 0,
                    TeleportPairCount = 0,

                    WallFill = wallFill,
                    MaxAttempts = 200,
                    MaxPlacementTries = 500,
                    Seed = seed,

                    EnemyUidPrefix = "SwordStatue",

                    CarveMaxSteps = 4,
                    CarveTargetRoadRatioX100 = -1, // -1이면 WallFill 기반으로 자동 계산
                    RequireNoDeadRoad = true,
                };
            }
        }

        // ------------------------------------------
        // Public Entry
        // ------------------------------------------
        public static MapData Generate(Config cfg)
        {
            var rng = new System.Random(cfg.Seed);

            for (int attempt = 0; attempt < Mathf.Max(1, cfg.MaxAttempts); attempt++)
            {
                // 1) Carve-based tiles (starts as ALL walls, then carve connected corridors)
                int[] tiles = BuildCarvedTiles(cfg, rng, out Tile player);

                if (player.Equals(Tile.Invalid))
                    continue;

                // 2) Place effects (on road, no overlap)
                var effects = new List<EffectObjectData>(
                    cfg.StopCount + cfg.TurnLeftCount + cfg.TurnRightCount + cfg.TeleportPairCount * 2);

                var enemies = new List<EnemySettingData>(cfg.EnemyCount);
                var occ = new HashSet<Tile>();
                occ.Add(player);

                if (!TryPlaceTeleports(cfg, tiles, player, occ, effects, rng)) continue;
                if (!TryPlaceTurns(cfg, tiles, player, occ, effects, rng)) continue;
                if (!TryPlaceStops(cfg, tiles, player, occ, effects, rng)) continue;

                // 3) Place enemies (must be killable from reachable end positions)
                if (!TryPlaceEnemiesIncremental(cfg, tiles, player, occ, effects, enemies, rng))
                    continue;

                // 4) Validate: no "dead road" if required
                if (cfg.RequireNoDeadRoad)
                {
                    if (!ValidateNoDeadRoadFromStart(cfg, tiles, player, effects, enemies))
                        continue;
                }

                // 5) Validate: all enemies killable from start
                if (!ValidateAllEnemiesKillable(cfg, tiles, player, effects, enemies))
                    continue;

                // 6) Build MapData
                var data = ScriptableObject.CreateInstance<MapData>();
                data.Width = cfg.Width;
                data.Height = cfg.Height;
                data.MapTiles = tiles;

                data.effectTileDatas = effects.ToArray();
                return data;
            }

            return BuildFallback(cfg);
        }

        // =========================================================
        //  CARVE MAP (ALL WALL -> CONNECTED CORRIDOR GROWTH)
        // =========================================================
        private static int[] BuildCarvedTiles(Config cfg, System.Random rng, out Tile player)
        {
            int w = cfg.Width;
            int h = cfg.Height;
            int[] tiles = new int[w * h];

            // 0) start as ALL walls
            for (int i = 0; i < tiles.Length; i++) tiles[i] = 1;

            // 1) Make borders walls explicitly (already 1)
            // 2) Decide target road count
            int interior = (w - 2) * (h - 2);
            int targetRoad;
            if (cfg.CarveTargetRoadRatioX100 > 0)
                targetRoad = Mathf.Clamp(interior * cfg.CarveTargetRoadRatioX100 / 100, 8, interior);
            else
            {
                // road ratio ~= (1 - wallFill)
                float wallFill = Mathf.Clamp(cfg.WallFill, 0f, 0.6f);
                float roadRatio = Mathf.Clamp01(1f - wallFill);
                targetRoad = Mathf.Clamp(Mathf.RoundToInt(interior * roadRatio), 8, interior);
            }

            // 3) Pick a seed tile adjacent to border (inside ring)
            //    (벽에 붙어있는 타일 중 랜덤 1개)
            var seed = PickBorderAdjacentInterior(cfg, rng);
            if (seed.Equals(Tile.Invalid))
            {
                player = Tile.Invalid;
                return tiles;
            }

            SetRoad(tiles, w, seed);
            int roadCount = 1;

            // "frontier ends": we grow corridors from these
            var ends = new List<Tile>(64) { seed };

            // 4) Grow corridors
            // - choose an end, carve a corridor in a direction
            // - replace that end with new end (like a growing snake)
            // - sometimes carve a side-branch to create junctions (avoid rail-only maps)
            int guard = 0;
            while (roadCount < targetRoad && ends.Count > 0 && guard++ < 5000)
            {
                // pick an end
                int ei = rng.Next(ends.Count);
                var from = ends[ei];

                // sometimes branch from a random road tile to create variety
                bool doBranch = (rng.NextDouble() < 0.30);
                if (doBranch)
                {
                    if (TryPickRandomRoadTile(cfg, tiles, rng, out var branchFrom))
                    {
                        if (TryCarve(cfg, tiles, rng, branchFrom, out var newEnd, out int carved))
                        {
                            roadCount += carved;
                            ends.Add(newEnd);
                            continue;
                        }
                    }
                }

                // normal growth from end
                if (TryCarve(cfg, tiles, rng, from, out var end2, out int carved2))
                {
                    roadCount += carved2;
                    ends[ei] = end2; // replace
                }
                else
                {
                    // dead end can't grow -> remove
                    ends.RemoveAt(ei);
                }
            }

            // 5) Keep only the largest connected road component (safety)
            KeepLargestRoadComponent(tiles, w, h);

            // 6) Pick player on road
            if (!TryPickRandomRoad(cfg, tiles, rng, out player))
                player = Tile.Invalid;

            return tiles;
        }

        private static Tile PickBorderAdjacentInterior(Config cfg, System.Random rng)
        {
            // interior ring: x==1 or x==w-2 or z==1 or z==h-2
            var list = new List<Tile>();
            int w = cfg.Width, h = cfg.Height;

            for (int x = 1; x < w - 1; x++)
            {
                list.Add(new Tile(x, 1));
                list.Add(new Tile(x, h - 2));
            }
            for (int z = 2; z < h - 2; z++)
            {
                list.Add(new Tile(1, z));
                list.Add(new Tile(w - 2, z));
            }

            if (list.Count == 0) return Tile.Invalid;
            return list[rng.Next(list.Count)];
        }

        private static bool TryCarve(Config cfg, int[] tiles, System.Random rng, Tile from, out Tile newEnd, out int carved)
        {
            carved = 0;
            newEnd = Tile.Invalid;

            // pick random direction order
            var dirs = new List<Dir> { Dir.Up, Dir.Right, Dir.Down, Dir.Left };
            Shuffle(dirs, rng);

            int w = cfg.Width, h = cfg.Height;

            foreach (var d in dirs)
            {
                var (dx, dz) = ToDelta(d);

                // we carve 2..max steps (short corridors are better for EndPos variety)
                int maxSteps = Mathf.Max(2, cfg.CarveMaxSteps);
                int steps = rng.Next(2, maxSteps + 1);

                // attempt carve
                Tile cur = from;
                int localCarved = 0;
                bool hitSomething = false;

                for (int i = 0; i < steps; i++)
                {
                    var nxt = new Tile(cur.X + dx, cur.Z + dz);

                    // stop if out of interior
                    if (nxt.X <= 0 || nxt.X >= w - 1 || nxt.Z <= 0 || nxt.Z >= h - 1)
                    {
                        hitSomething = true;
                        break;
                    }

                    int idx = nxt.Z * w + nxt.X;

                    // if meets existing road, allow "loop connect" but stop carving further
                    if (tiles[idx] == 0)
                    {
                        hitSomething = true;
                        break;
                    }

                    // carve
                    tiles[idx] = 0;
                    localCarved++;
                    cur = nxt;
                }

                if (localCarved == 0) continue;

                // Ensure it becomes an "end" in that direction: next cell after cur must be blocked or boundary
                var after = new Tile(cur.X + dx, cur.Z + dz);
                if (after.X <= 0 || after.X >= w - 1 || after.Z <= 0 || after.Z >= h - 1)
                {
                    // boundary is blocked -> ok
                }
                else
                {
                    int aidx = after.Z * w + after.X;
                    if (tiles[aidx] == 0)
                    {
                        // connected into existing road: still ok, but end might not be end.
                        // We accept; end will be validated later by "no dead road" check anyway.
                    }
                    else
                    {
                        // it's wall, good
                    }
                }

                carved = localCarved;
                newEnd = cur;
                return true;
            }

            return false;
        }

        private static void SetRoad(int[] tiles, int w, Tile t)
        {
            tiles[t.Z * w + t.X] = 0;
        }

        // =========================================================
        //  VALIDATION: "NO DEAD ROAD" (passable OR stoppable)
        // =========================================================
        private static bool ValidateNoDeadRoadFromStart(
            Config cfg, int[] tiles, Tile player,
            List<EffectObjectData> effects,
            List<EnemySettingData> enemies)
        {
            var effectByPos = BuildEffectLookup(effects);
            var enemyPos = BuildEnemyPosSet(enemies);

            if (!ComputeReachableEndsAndPaths(cfg, tiles, player, effectByPos, enemyPos,
                    out var ends, out var paths))
                return false;

            var meaningful = new HashSet<Tile>(ends);
            foreach (var t in paths) meaningful.Add(t);

            // protected: enemy/effects/player are not required to be "road meaningful"
            var protectedTiles = new HashSet<Tile>(enemyPos) { player };
            for (int i = 0; i < effects.Count; i++)
                protectedTiles.Add(effects[i].Point);

            // every road tile must be meaningful
            int w = cfg.Width, h = cfg.Height;
            for (int z = 1; z < h - 1; z++)
                for (int x = 1; x < w - 1; x++)
                {
                    int idx = z * w + x;
                    if (tiles[idx] != 0) continue;

                    var t = new Tile(x, z);
                    if (protectedTiles.Contains(t)) continue;

                    if (!meaningful.Contains(t))
                        return false;
                }

            return true;
        }

        private static bool ValidateAllEnemiesKillable(
            Config cfg, int[] tiles, Tile player,
            List<EffectObjectData> effects, List<EnemySettingData> enemies)
        {
            var effectByPos = BuildEffectLookup(effects);
            var enemyPos = BuildEnemyPosSet(enemies);

            if (!ComputeReachableEnds(cfg, tiles, player, effectByPos, enemyPos, out var reachableEnds))
                return false;

            return AllEnemiesKillableWithReach(cfg, enemyPos, tiles, reachableEnds);
        }

        // =========================================================
        //  Keep largest connected road component (4-neighbor)
        // =========================================================
        private static void KeepLargestRoadComponent(int[] tiles, int w, int h)
        {
            bool In(int x, int z) => x >= 0 && x < w && z >= 0 && z < h;
            bool IsRoad(int x, int z) => tiles[z * w + x] == 0;

            var visited = new bool[w * h];
            List<int> best = null;
            var q = new Queue<(int x, int z)>();

            for (int z = 1; z < h - 1; z++)
                for (int x = 1; x < w - 1; x++)
                {
                    int startIdx = z * w + x;
                    if (visited[startIdx]) continue;
                    if (!IsRoad(x, z)) continue;

                    var comp = new List<int>(64);
                    visited[startIdx] = true;
                    q.Enqueue((x, z));

                    while (q.Count > 0)
                    {
                        var (cx, cz) = q.Dequeue();
                        int cidx = cz * w + cx;
                        comp.Add(cidx);

                        var dirs = new (int dx, int dz)[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
                        foreach (var (dx, dz) in dirs)
                        {
                            int nx = cx + dx, nz = cz + dz;
                            if (!In(nx, nz)) continue;
                            int nidx = nz * w + nx;
                            if (visited[nidx]) continue;
                            if (!IsRoad(nx, nz)) continue;

                            visited[nidx] = true;
                            q.Enqueue((nx, nz));
                        }
                    }

                    if (best == null || comp.Count > best.Count)
                        best = comp;
                }

            if (best == null) return;

            var keep = new bool[w * h];
            for (int i = 0; i < best.Count; i++) keep[best[i]] = true;

            for (int z = 1; z < h - 1; z++)
                for (int x = 1; x < w - 1; x++)
                {
                    int idx = z * w + x;
                    if (tiles[idx] == 0 && !keep[idx])
                        tiles[idx] = 1;
                }
        }

        // =========================================================
        //  Effect placement (same rules as before)
        // =========================================================
        private static bool TryPickRandomRoad(Config cfg, int[] tiles, System.Random rng, out Tile pos)
        {
            for (int i = 0; i < cfg.MaxPlacementTries; i++)
            {
                int x = rng.Next(1, cfg.Width - 1);
                int z = rng.Next(1, cfg.Height - 1);
                var t = new Tile(x, z);
                if (IsRoad(cfg, tiles, t))
                {
                    pos = t;
                    return true;
                }
            }
            pos = Tile.Invalid;
            return false;
        }

        private static bool TryPickRandomRoadTile(Config cfg, int[] tiles, System.Random rng, out Tile pos)
        {
            // shorter tries for carving
            for (int i = 0; i < 128; i++)
            {
                int x = rng.Next(1, cfg.Width - 1);
                int z = rng.Next(1, cfg.Height - 1);
                if (tiles[z * cfg.Width + x] == 0)
                {
                    pos = new Tile(x, z);
                    return true;
                }
            }
            pos = Tile.Invalid;
            return false;
        }

        private static bool TryPickEffectSpot(
            Config cfg, int[] tiles, Tile player, HashSet<Tile> occ, System.Random rng,
            int minDistFromPlayer, out Tile p)
        {
            for (int i = 0; i < cfg.MaxPlacementTries; i++)
            {
                int x = rng.Next(1, cfg.Width - 1);
                int z = rng.Next(1, cfg.Height - 1);
                var t = new Tile(x, z);

                if (!IsRoad(cfg, tiles, t)) continue;
                if (occ.Contains(t)) continue;
                if (Manhattan(t, player) < minDistFromPlayer) continue;

                p = t;
                return true;
            }

            p = Tile.Invalid;
            return false;
        }

        private static bool TryPlaceTeleports(
            Config cfg, int[] tiles, Tile player, HashSet<Tile> occ,
            List<EffectObjectData> effects, System.Random rng)
        {
            for (int pair = 0; pair < cfg.TeleportPairCount; pair++)
            {
                if (!TryPickEffectSpot(cfg, tiles, player, occ, rng, 3, out Tile a)) return false;
                occ.Add(a);

                if (!TryPickEffectSpot(cfg, tiles, player, occ, rng, 3, out Tile b)) return false;
                occ.Add(b);

                var ea = new EffectObjectData(a, EEffectObjectType.Teleport) { TeleportPoint = b };
                var eb = new EffectObjectData(b, EEffectObjectType.Teleport) { TeleportPoint = a };
                effects.Add(ea);
                effects.Add(eb);
            }
            return true;
        }

        private static bool TryPlaceTurns(
            Config cfg, int[] tiles, Tile player, HashSet<Tile> occ,
            List<EffectObjectData> effects, System.Random rng)
        {
            for (int i = 0; i < cfg.TurnLeftCount; i++)
            {
                if (!TryPickEffectSpot(cfg, tiles, player, occ, rng, 2, out Tile p)) return false;
                if (IsTurnAdjacent(effects, p)) { i--; continue; }
                occ.Add(p);
                effects.Add(new EffectObjectData(p, EEffectObjectType.TurnLeft));
            }

            for (int i = 0; i < cfg.TurnRightCount; i++)
            {
                if (!TryPickEffectSpot(cfg, tiles, player, occ, rng, 2, out Tile p)) return false;
                if (IsTurnAdjacent(effects, p)) { i--; continue; }
                occ.Add(p);
                effects.Add(new EffectObjectData(p, EEffectObjectType.TurnRight));
            }

            return true;
        }

        private static bool TryPlaceStops(
            Config cfg, int[] tiles, Tile player, HashSet<Tile> occ,
            List<EffectObjectData> effects, System.Random rng)
        {
            for (int i = 0; i < cfg.StopCount; i++)
            {
                if (!TryPickEffectSpot(cfg, tiles, player, occ, rng, 1, out Tile p)) return false;
                occ.Add(p);
                effects.Add(new EffectObjectData(p, EEffectObjectType.Stop));
            }
            return true;
        }

        private static bool IsTurnAdjacent(List<EffectObjectData> effects, Tile p)
        {
            foreach (var e in effects)
            {
                if (e.EffectObjectType == EEffectObjectType.TurnLeft ||
                    e.EffectObjectType == EEffectObjectType.TurnRight)
                {
                    if (Mathf.Abs(e.Point.X - p.X) + Mathf.Abs(e.Point.Z - p.Z) <= 1)
                        return true;
                }
            }
            return false;
        }

        // =========================================================
        //  Enemy placement (incremental validation)
        // =========================================================
        private static bool TryPlaceEnemiesIncremental(
            Config cfg, int[] tiles, Tile player, HashSet<Tile> occ,
            List<EffectObjectData> effects, List<EnemySettingData> enemies, System.Random rng)
        {
            var effectByPos = BuildEffectLookup(effects);

            for (int i = 0; i < cfg.EnemyCount; i++)
            {
                bool placed = false;
                var enemyPosSet = BuildEnemyPosSet(enemies);

                if (!ComputeReachableEnds(cfg, tiles, player, effectByPos, enemyPosSet, out var reachableEnds))
                    return false;

                for (int t = 0; t < cfg.MaxPlacementTries; t++)
                {
                    int x = rng.Next(1, cfg.Width - 1);
                    int z = rng.Next(1, cfg.Height - 1);
                    var c = new Tile(x, z);

                    if (!IsRoad(cfg, tiles, c)) continue;
                    if (occ.Contains(c)) continue;

                    // must have at least one adjacent standable road tile
                    bool hasAdjStand = false;
                    foreach (var adj in Neighbors4(c))
                    {
                        if (!InBounds(cfg, adj)) continue;
                        if (IsRoad(cfg, tiles, adj) && !enemyPosSet.Contains(adj))
                        {
                            hasAdjStand = true;
                            break;
                        }
                    }
                    if (!hasAdjStand) continue;

                    // prefer adjacent to a reachable end (so killable)
                    bool adjEnd = false;
                    foreach (var adj in Neighbors4(c))
                    {
                        if (reachableEnds.Contains(adj))
                        {
                            adjEnd = true;
                            break;
                        }
                    }
                    if (!adjEnd && rng.NextDouble() < 0.80) continue;

                    // tentative
                    enemyPosSet.Add(c);
                    occ.Add(c);

                    if (!ComputeReachableEnds(cfg, tiles, player, effectByPos, enemyPosSet, out var afterEnds))
                    {
                        enemyPosSet.Remove(c);
                        occ.Remove(c);
                        continue;
                    }

                    if (!AllEnemiesKillableWithReach(cfg, enemyPosSet, tiles, afterEnds))
                    {
                        enemyPosSet.Remove(c);
                        occ.Remove(c);
                        continue;
                    }

                    enemies.Add(new EnemySettingData(new Tile(0,0)));
                    placed = true;
                    break;
                }

                if (!placed) return false;
            }

            return true;
        }

        private static bool AllEnemiesKillableWithReach(
            Config cfg, HashSet<Tile> enemyPos, int[] tiles, HashSet<Tile> reachableEnds)
        {
            foreach (var e in enemyPos)
            {
                bool canAttack = false;
                foreach (var adj in Neighbors4(e))
                {
                    if (!InBounds(cfg, adj)) continue;
                    if (tiles[adj.Z * cfg.Width + adj.X] != 0) continue;
                    if (enemyPos.Contains(adj)) continue;
                    if (reachableEnds.Contains(adj))
                    {
                        canAttack = true;
                        break;
                    }
                }
                if (!canAttack) return false;
            }
            return true;
        }

        // =========================================================
        //  Sliding Reachability (Ends + Paths)
        // =========================================================
        private enum Dir { Up, Right, Down, Left }
        private static readonly Dir[] AllDirs = { Dir.Up, Dir.Right, Dir.Down, Dir.Left };

        private static bool ComputeReachableEnds(
            Config cfg, int[] tiles, Tile start,
            Dictionary<Tile, EffectObjectData> effectByPos,
            HashSet<Tile> enemyPos,
            out HashSet<Tile> reachableEnds)
        {
            reachableEnds = new HashSet<Tile>();
            var q = new Queue<Tile>();

            reachableEnds.Add(start);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                foreach (var d in AllDirs)
                {
                    var res = SimulateMove(cfg, tiles, cur, d, effectByPos, enemyPos);
                    if (!res.IsValid) return false;

                    if (reachableEnds.Add(res.EndPos))
                        q.Enqueue(res.EndPos);
                }
            }

            return true;
        }

        private static bool ComputeReachableEndsAndPaths(
            Config cfg, int[] tiles, Tile start,
            Dictionary<Tile, EffectObjectData> effectByPos,
            HashSet<Tile> enemyPos,
            out HashSet<Tile> reachableEnds,
            out HashSet<Tile> reachablePathTiles)
        {
            reachableEnds = new HashSet<Tile>();
            reachablePathTiles = new HashSet<Tile>();

            var q = new Queue<Tile>();
            reachableEnds.Add(start);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                foreach (var d in AllDirs)
                {
                    var trace = new HashSet<Tile>();
                    var res = SimulateMoveWithTrace(cfg, tiles, cur, d, effectByPos, enemyPos, trace);
                    if (!res.IsValid) return false;

                    foreach (var t in trace) reachablePathTiles.Add(t);

                    if (reachableEnds.Add(res.EndPos))
                        q.Enqueue(res.EndPos);
                }
            }

            return true;
        }

        private readonly struct SimResult
        {
            public readonly bool IsValid;
            public readonly Tile EndPos;
            public SimResult(bool valid, Tile endPos) { IsValid = valid; EndPos = endPos; }
            public static SimResult Invalid => new SimResult(false, Tile.Invalid);
            public static SimResult End(Tile p) => new SimResult(true, p);
        }

        private static SimResult SimulateMove(
            Config cfg, int[] tiles, Tile start, Dir inputDir,
            Dictionary<Tile, EffectObjectData> effectByPos,
            HashSet<Tile> enemyPos)
        {
            return SimulateMoveWithTrace(cfg, tiles, start, inputDir, effectByPos, enemyPos, null);
        }

        private static SimResult SimulateMoveWithTrace(
            Config cfg, int[] tiles, Tile start, Dir inputDir,
            Dictionary<Tile, EffectObjectData> effectByPos,
            HashSet<Tile> enemyPos,
            HashSet<Tile> trace)
        {
            Tile pos = start;
            Dir dir = inputDir;

            var visited = new HashSet<(Tile, Dir)>();
            visited.Add((pos, dir));

            while (true)
            {
                var (dx, dz) = ToDelta(dir);
                var next = new Tile(pos.X + dx, pos.Z + dz);

                if (IsBlocked(cfg, tiles, next, enemyPos))
                    return SimResult.End(pos);

                pos = next;
                trace?.Add(pos);

                if (effectByPos.TryGetValue(pos, out var eff))
                {
                    switch (eff.EffectObjectType)
                    {
                        case EEffectObjectType.Stop:
                            return SimResult.End(pos);

                        case EEffectObjectType.TurnLeft:
                            dir = TurnLeft(dir);
                            break;

                        case EEffectObjectType.TurnRight:
                            dir = TurnRight(dir);
                            break;

                        case EEffectObjectType.Teleport:
                            {
                                var dest = eff.TeleportPoint;
                                if (dest.Equals(Tile.Invalid)) return SimResult.Invalid;
                                if (IsBlocked(cfg, tiles, dest, enemyPos)) return SimResult.Invalid;
                                pos = dest;
                                trace?.Add(pos);
                                break;
                            }
                    }
                }

                if (!visited.Add((pos, dir)))
                    return SimResult.Invalid;
            }
        }

        private static bool IsBlocked(Config cfg, int[] tiles, Tile t, HashSet<Tile> enemyPos)
        {
            if (t.X < 0 || t.X >= cfg.Width || t.Z < 0 || t.Z >= cfg.Height)
                return true;

            if (tiles[t.Z * cfg.Width + t.X] == 1)
                return true;

            if (enemyPos.Contains(t))
                return true;

            return false;
        }

        private static (int dx, int dz) ToDelta(Dir d) => d switch
        {
            Dir.Up => (0, 1),
            Dir.Right => (1, 0),
            Dir.Down => (0, -1),
            Dir.Left => (-1, 0),
            _ => (0, 0)
        };

        private static Dir TurnLeft(Dir d) => (Dir)(((int)d + 3) % 4);
        private static Dir TurnRight(Dir d) => (Dir)(((int)d + 1) % 4);

        // =========================================================
        //  Generic helpers
        // =========================================================
        private static bool InBounds(Config cfg, Tile t)
            => t.X >= 0 && t.X < cfg.Width && t.Z >= 0 && t.Z < cfg.Height;

        private static bool IsRoad(Config cfg, int[] tiles, Tile t)
            => InBounds(cfg, t) && tiles[t.Z * cfg.Width + t.X] == 0;

        private static int Manhattan(Tile a, Tile b)
            => Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Z - b.Z);

        private static IEnumerable<Tile> Neighbors4(Tile t)
        {
            yield return new Tile(t.X, t.Z + 1);
            yield return new Tile(t.X + 1, t.Z);
            yield return new Tile(t.X, t.Z - 1);
            yield return new Tile(t.X - 1, t.Z);
        }

        private static Dictionary<Tile, EffectObjectData> BuildEffectLookup(List<EffectObjectData> effects)
        {
            var dict = new Dictionary<Tile, EffectObjectData>(effects.Count);
            for (int i = 0; i < effects.Count; i++)
                dict[effects[i].Point] = effects[i];
            return dict;
        }

        private static HashSet<Tile> BuildEnemyPosSet(List<EnemySettingData> enemies)
        {
            var set = new HashSet<Tile>();
            for (int i = 0; i < enemies.Count; i++)
                set.Add(enemies[i].Point);
            return set;
        }

        private static void Shuffle<T>(List<T> list, System.Random rng)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        // =========================================================
        //  Fallback
        // =========================================================
        private static MapData BuildFallback(Config cfg)
        {
            int w = Mathf.Max(5, cfg.Width);
            int h = Mathf.Max(5, cfg.Height);

            var tiles = new int[w * h];
            for (int z = 0; z < h; z++)
                for (int x = 0; x < w; x++)
                {
                    bool border = (x == 0 || z == 0 || x == w - 1 || z == h - 1);
                    tiles[z * w + x] = border ? 1 : 0;
                }

            var data = ScriptableObject.CreateInstance<MapData>();
            data.Width = w;
            data.Height = h;
            data.MapTiles = tiles;
           
            data.effectTileDatas = Array.Empty<EffectObjectData>();
            return data;
        }
    }
}
