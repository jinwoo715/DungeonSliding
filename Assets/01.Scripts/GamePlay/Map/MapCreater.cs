using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    // 0 = Road, 1 = Wall
    // Enemy/StopTile을 벽 취급하려면 IsBlocker 판정만 확장하면 됨.
    public static class MapCreater
    {
        public enum Dir { Up, Right, Down, Left }

        public struct Tile
        {
            public int X;
            public int Z;
            public Tile(int x, int z) { X = x; Z = z; }
            public override string ToString() => $"({X},{Z})";
        }

        private static readonly (int dx, int dz)[] D =
        {
            (0, 1),   // Up
            (1, 0),   // Right
            (0, -1),  // Down
            (-1, 0),  // Left
        };

        private struct Frontier
        {
            public Tile FromStop;
            public int FromStopIdx;
            public Dir Dir;
            public List<Tile> LineCells;  // 인접칸 제외된 라인 후보
            public int Bucket;
        }

        // ===== 튜닝 =====
        private static int ComputeBucketAxis(int len)
        {
            if (len <= 6) return 3;
            if (len <= 10) return 4;
            if (len <= 14) return 5;
            return 6;
        }

        private const float BucketWallPower = 2.3f;
        private const float BucketCandPower = 0.35f;

        private const int DensityRadius = 2;
        private const float DensityPower = 2.2f;

        private const float GlobalDirPower = 1.8f;
        private const float PerStopDirPower = 2.2f;

        private const bool FirstWallMustAlignToStartRowCol = true;

        // “이 벽이 의미 있어야 한다” 체크를 위한 재시도
        private const int MaxPlacementTriesPerWall = 60;

        // 맵 자체를 리롤해서라도 “start→goal 가능”을 보장
        private const int MaxMapRerolls = 30;

        /// <summary>
        /// goal까지 도달 가능한 맵만 반환(보장).
        /// </summary>
        public static MapData CreateMap(int width, int height, int wallCount, Tile playerStart, Tile goal, int? seed = null)
        {
            // 좌표가 유효하지 않으면 중앙으로 보정
            if (!In(playerStart.X, playerStart.Z, width, height))
                playerStart = new Tile(width / 2, height / 2);

            if (!In(goal.X, goal.Z, width, height))
                goal = new Tile(width - 2, height - 2);

            // goal이나 start가 외곽 벽으로 취급될 수 있으면(네 룰에 따라) 여기서 방지
            if (playerStart.X == goal.X && playerStart.Z == goal.Z)
                goal = new Tile(Mathf.Clamp(goal.X + 1, 0, width - 1), goal.Z);

            // 리롤 루프
            for (int reroll = 0; reroll < MaxMapRerolls; reroll++)
            {
                if (seed.HasValue)
                    UnityEngine.Random.InitState(seed.Value + reroll);

                var map = new int[height, width]; // 0 road, 1 wall

                // start/goal은 길 강제
                map[playerStart.Z, playerStart.X] = 0;
                map[goal.Z, goal.X] = 0;

                int sx = ComputeBucketAxis(width);
                int sz = ComputeBucketAxis(height);
                int bucketTotal = sx * sz;

                var wallsByBucket = new int[bucketTotal];
                var globalDirUsed = new int[4];
                var perStopDirUsed = new Dictionary<int, int>(); // stopIdx -> bitmask

                // 1) 첫 벽
                if (wallCount > 0)
                {
                    Tile firstWall = FirstWallMustAlignToStartRowCol
                        ? PickFirstBorderWallAligned(width, height, playerStart)
                        : PickRandomBorderWall(width, height);

                    // start/goal 위에는 벽 금지
                    if (!Same(firstWall, playerStart) && !Same(firstWall, goal))
                    {
                        map[firstWall.Z, firstWall.X] = 1;
                        wallsByBucket[GetBucket(firstWall.X, firstWall.Z, width, height, sx, sz)]++;
                    }
                }

                // 처음부터 goal 불가능이면 리롤
                if (!CanReach(map, width, height, playerStart, goal))
                    continue;

                bool failed = false;

                // 2) 벽 배치 반복
                for (int i = 1; i < wallCount; i++)
                {
                    var reachableBefore = ComputeReachableStops(map, width, height, playerStart);
                    if (reachableBefore.Count == 0) { failed = true; break; }

                    var frontiers = BuildFrontiers(map, width, height, reachableBefore, sx, sz);
                    if (frontiers.Count == 0) { failed = true; break; }

                    bool placed = false;

                    // 후보 재시도
                    for (int attempt = 0; attempt < MaxPlacementTriesPerWall; attempt++)
                    {
                        int pickedIndex = PickFrontierWeighted(frontiers, wallsByBucket, globalDirUsed, perStopDirUsed);
                        var f = frontiers[pickedIndex];

                        Tile wallPos = PickCellSpreadWeighted(map, width, height, f.LineCells, DensityRadius, DensityPower);
                        if (!In(wallPos.X, wallPos.Z, width, height)) continue;
                        if (IsBlocker(map, wallPos)) continue;
                        if (Same(wallPos, playerStart) || Same(wallPos, goal)) continue;

                        // ---- 가상 배치 ----
                        map[wallPos.Z, wallPos.X] = 1;

                        // (A) goal 도달 가능 유지(최우선)
                        if (!CanReach(map, width, height, playerStart, goal))
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        // (B) 그래프 끊김 방지: fromStop은 계속 reachable
                        var reachableAfter = ComputeReachableStops(map, width, height, playerStart);
                        if (!reachableAfter.Contains(f.FromStopIdx))
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        // (C) 새 stop 생성되어야 함(벽 놓았는데 여전히 못 멈추면 무효)
                        Tile newStop = SlideToStopIfBlocked(map, width, height, f.FromStop, f.Dir);
                        if (newStop.X == int.MinValue)
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        int newIdx = newStop.Z * width + newStop.X;

                        // (D) newStop도 reachable이어야 의미 있음
                        if (!reachableAfter.Contains(newIdx))
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        // (E) “존재 이유 없는 벽” 제거: newStop이 이미 있었으면 버림
                        if (reachableBefore.Contains(newIdx))
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        // (F) 가능하면 순증가 강제(너가 싫어하는 ‘중복 역할 벽’ 많이 줄어듦)
                        if (reachableAfter.Count <= reachableBefore.Count)
                        {
                            map[wallPos.Z, wallPos.X] = 0;
                            continue;
                        }

                        // ---- 확정 배치(가상 배치 유지) ----
                        int bw = GetBucket(wallPos.X, wallPos.Z, width, height, sx, sz);
                        wallsByBucket[bw]++;

                        globalDirUsed[(int)f.Dir]++;

                        int mask = perStopDirUsed.TryGetValue(f.FromStopIdx, out var m) ? m : 0;
                        mask |= (1 << (int)f.Dir);
                        perStopDirUsed[f.FromStopIdx] = mask;

                        placed = true;
                        break;
                    }

                    if (!placed) { failed = true; break; }
                }

                // 최종 검증: goal 도달 가능 아니면 리롤
                if (failed) continue;
                if (!CanReach(map, width, height, playerStart, goal)) continue;

                // MapData 변환
                int[] tiles = new int[width * height];
                for (int z = 0; z < height; z++)
                    for (int x = 0; x < width; x++)
                        tiles[z * width + x] = map[z, x];

                var data = ScriptableObject.CreateInstance<MapData>();
                data.Width = width;
                data.Height = height;
                //data.MapTiles = tiles;
                return data;
            }

            // 여기까지 왔다는 건 “주어진 wallCount/크기”로는 제약이 너무 빡세서 못 만들었다는 뜻.
            // (예: 벽이 너무 많거나, start/goal 배치가 너무 빡빡)
            throw new Exception($"CreateMap failed: could not generate solvable map after {MaxMapRerolls} rerolls. " +
                                $"(w={width}, h={height}, walls={wallCount}, start={playerStart}, goal={goal})");
        }

        // ===== Reachability(Goal 보장 핵심) =====
        private static bool CanReach(int[,] map, int w, int h, Tile start, Tile goal)
        {
            if (!In(start.X, start.Z, w, h) || !In(goal.X, goal.Z, w, h)) return false;
            if (IsBlocker(map, start) || IsBlocker(map, goal)) return false;

            int s = start.Z * w + start.X;
            int t = goal.Z * w + goal.X;

            var q = new Queue<Tile>();
            var vis = new HashSet<int>();

            vis.Add(s);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                int curIdx = cur.Z * w + cur.X;
                if (curIdx == t) return true;

                for (int di = 0; di < 4; di++)
                {
                    var next = SlideToStopIfBlocked(map, w, h, cur, (Dir)di);
                    if (next.X == int.MinValue) continue;

                    int idx = next.Z * w + next.X;
                    if (vis.Add(idx))
                        q.Enqueue(next);
                }
            }

            return false;
        }

        // ===== Reachable Stops (sliding BFS) =====
        private static HashSet<int> ComputeReachableStops(int[,] map, int w, int h, Tile start)
        {
            var visited = new HashSet<int>();
            var q = new Queue<Tile>();

            if (!In(start.X, start.Z, w, h)) return visited;
            if (IsBlocker(map, start)) return visited;

            int sIdx = start.Z * w + start.X;
            visited.Add(sIdx);
            q.Enqueue(start);

            while (q.Count > 0)
            {
                var cur = q.Dequeue();
                for (int di = 0; di < 4; di++)
                {
                    var next = SlideToStopIfBlocked(map, w, h, cur, (Dir)di);
                    if (next.X == int.MinValue) continue;

                    int idx = next.Z * w + next.X;
                    if (visited.Add(idx))
                        q.Enqueue(next);
                }
            }

            return visited;
        }

        // “벽을 만나야 멈출 수 있다” 규칙 그대로
        // + 바로 옆이 벽이면 0칸 이동이라 이동 무효
        private static Tile SlideToStopIfBlocked(int[,] map, int w, int h, Tile from, Dir dir)
        {
            var (dx, dz) = D[(int)dir];

            int lastX = from.X;
            int lastZ = from.Z;

            int nx = lastX + dx;
            int nz = lastZ + dz;

            if (!In(nx, nz, w, h)) return new Tile(int.MinValue, int.MinValue);
            if (map[nz, nx] == 1) return new Tile(int.MinValue, int.MinValue);

            while (true)
            {
                int tx = lastX + dx;
                int tz = lastZ + dz;

                if (!In(tx, tz, w, h))
                    return new Tile(int.MinValue, int.MinValue); // blocker 못 만나면 무효

                if (map[tz, tx] == 1)
                    return new Tile(lastX, lastZ); // blocker 직전에서 stop

                lastX = tx;
                lastZ = tz;
            }
        }

        // ===== Frontiers =====
        private static List<Frontier> BuildFrontiers(int[,] map, int w, int h, HashSet<int> reachableStops, int sx, int sz)
        {
            var list = new List<Frontier>();

            foreach (var idx in reachableStops)
            {
                int z = idx / w;
                int x = idx % w;
                var from = new Tile(x, z);

                for (int di = 0; di < 4; di++)
                {
                    Dir dir = (Dir)di;

                    // 이미 blocker가 앞에 있으면 이동 가능 → frontier 아님
                    if (HasBlockerAhead(map, w, h, from, dir))
                        continue;

                    var line = CollectLineCells(map, w, h, from, dir);
                    if (line.Count == 0) continue;

                    int bucket = GetBucket(from.X, from.Z, w, h, sx, sz);

                    list.Add(new Frontier
                    {
                        FromStop = from,
                        FromStopIdx = idx,
                        Dir = dir,
                        LineCells = line,
                        Bucket = bucket
                    });
                }
            }

            return list;
        }

        private static bool HasBlockerAhead(int[,] map, int w, int h, Tile from, Dir dir)
        {
            var (dx, dz) = D[(int)dir];
            int x = from.X;
            int z = from.Z;

            while (true)
            {
                x += dx; z += dz;
                if (!In(x, z, w, h)) return false;
                if (map[z, x] == 1) return true;
            }
        }

        // 인접칸(step==1) 제외: 0칸 이동/무효 stop 방지
        private static List<Tile> CollectLineCells(int[,] map, int w, int h, Tile from, Dir dir)
        {
            var (dx, dz) = D[(int)dir];
            var line = new List<Tile>();

            int x = from.X;
            int z = from.Z;
            int step = 0;

            while (true)
            {
                x += dx; z += dz;
                step++;

                if (!In(x, z, w, h)) break;
                if (map[z, x] == 1) break;

                if (step >= 2) // 인접칸 제외
                    line.Add(new Tile(x, z));
            }

            return line;
        }

        // ===== Pick Frontier (bucket + dir diversity) =====
        private static int PickFrontierWeighted(
            List<Frontier> frontiers,
            int[] wallsByBucket,
            int[] globalDirUsed,
            Dictionary<int, int> perStopDirUsed)
        {
            // 버킷별 후보수
            var candByBucket = new Dictionary<int, int>();
            for (int i = 0; i < frontiers.Count; i++)
            {
                int b = frontiers[i].Bucket;
                candByBucket[b] = candByBucket.TryGetValue(b, out var c) ? (c + 1) : 1;
            }

            float total = 0f;
            var weights = new float[frontiers.Count];

            for (int i = 0; i < frontiers.Count; i++)
            {
                var f = frontiers[i];

                int b = f.Bucket;
                int bWalls = wallsByBucket[b];
                int bCand = candByBucket[b];

                float wBucket = Mathf.Pow(bCand + 1f, BucketCandPower) / Mathf.Pow(bWalls + 1f, BucketWallPower);

                int dUsed = globalDirUsed[(int)f.Dir];
                float wDir = 1f / Mathf.Pow(dUsed + 1f, GlobalDirPower);

                int mask = perStopDirUsed.TryGetValue(f.FromStopIdx, out var m) ? m : 0;
                bool usedHere = (mask & (1 << (int)f.Dir)) != 0;
                float wPerStop = usedHere ? (1f / Mathf.Pow(2f, PerStopDirPower)) : 1f;

                float w = wBucket * wDir * wPerStop;
                if (w < 0.0001f) w = 0.0001f;

                weights[i] = w;
                total += w;
            }

            float r = UnityEngine.Random.Range(0f, total);
            float acc = 0f;
            for (int i = 0; i < weights.Length; i++)
            {
                acc += weights[i];
                if (r <= acc) return i;
            }
            return weights.Length - 1;
        }

        // ===== Pick Cell on Line (spread) =====
        private static Tile PickCellSpreadWeighted(int[,] map, int w, int h, List<Tile> cells, int radius, float power)
        {
            float total = 0f;
            var weights = new float[cells.Count];

            for (int i = 0; i < cells.Count; i++)
            {
                var t = cells[i];
                if (!In(t.X, t.Z, w, h) || IsBlocker(map, t))
                {
                    weights[i] = 0f;
                    continue;
                }

                int dens = CountWallsInRadius(map, w, h, t, radius);
                float wgt = 1f / Mathf.Pow(1f + dens, power);
                if (wgt < 0.0001f) wgt = 0.0001f;

                weights[i] = wgt;
                total += wgt;
            }

            if (total <= 0f)
                return cells[UnityEngine.Random.Range(0, cells.Count)];

            float r = UnityEngine.Random.Range(0f, total);
            float acc = 0f;
            for (int i = 0; i < cells.Count; i++)
            {
                acc += weights[i];
                if (r <= acc) return cells[i];
            }

            return cells[cells.Count - 1];
        }

        private static int CountWallsInRadius(int[,] map, int w, int h, Tile c, int r)
        {
            int count = 0;
            int minX = Mathf.Max(0, c.X - r);
            int maxX = Mathf.Min(w - 1, c.X + r);
            int minZ = Mathf.Max(0, c.Z - r);
            int maxZ = Mathf.Min(h - 1, c.Z + r);

            for (int z = minZ; z <= maxZ; z++)
                for (int x = minX; x <= maxX; x++)
                {
                    if (x == c.X && z == c.Z) continue;
                    if (map[z, x] == 1) count++;
                }
            return count;
        }

        // ===== First wall helpers =====
        private static Tile PickFirstBorderWallAligned(int w, int h, Tile start)
        {
            var opts = new List<Tile>(4);
            if (start.Z != 0) opts.Add(new Tile(start.X, 0));
            if (start.Z != h - 1) opts.Add(new Tile(start.X, h - 1));
            if (start.X != 0) opts.Add(new Tile(0, start.Z));
            if (start.X != w - 1) opts.Add(new Tile(w - 1, start.Z));

            if (opts.Count == 0) return PickRandomBorderWall(w, h);
            return opts[UnityEngine.Random.Range(0, opts.Count)];
        }

        private static Tile PickRandomBorderWall(int w, int h)
        {
            var border = new List<Tile>();
            for (int x = 0; x < w; x++) { border.Add(new Tile(x, 0)); border.Add(new Tile(x, h - 1)); }
            for (int z = 1; z < h - 1; z++) { border.Add(new Tile(0, z)); border.Add(new Tile(w - 1, z)); }
            return border[UnityEngine.Random.Range(0, border.Count)];
        }

        // ===== Bucket / Utils =====
        private static int GetBucket(int x, int z, int w, int h, int sx, int sz)
        {
            int bx = Mathf.Clamp((x * sx) / w, 0, sx - 1);
            int bz = Mathf.Clamp((z * sz) / h, 0, sz - 1);
            return bz * sx + bx;
        }

        private static bool IsBlocker(int[,] map, Tile t) => map[t.Z, t.X] == 1;

        private static bool In(int x, int z, int w, int h)
            => x >= 0 && x < w && z >= 0 && z < h;

        private static bool Same(Tile a, Tile b) => a.X == b.X && a.Z == b.Z;
    }
}
