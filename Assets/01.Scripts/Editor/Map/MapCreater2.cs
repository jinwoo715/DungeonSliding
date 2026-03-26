using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    /// <summary>
    /// Sliding 퍼즐 전용 맵 생성기
    /// - 후보는 "플레이어 start에서 도달 가능한 정지점(reachable stops)"만 사용
    /// - 벽은 "정지점에서 벽이 없는 방향의 직선(line) 위"에만 생성(= frontier 확장)
    /// - 섹션은 4분면이 아니라 버킷(sx x sz)로 분할하여 분산
    /// - 버킷 내에서도 근처 벽 밀집(density) 낮은 위치 우선
    /// </summary>
    public static class MapCreater2
    {
        public enum Dir { Up, Right, Down, Left }

        // 0=길, 1=벽
        // Enemy/StopTile까지 포함하려면 IsBlocker 판정만 확장하면 됨.

        public struct Tile
        {
            public int X;
            public int Z;
            public Tile(int x, int z) { X = x; Z = z; }
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
            public Dir Dir;
            public List<Tile> LineCells; // FromStop에서 해당 방향으로 "벽을 놓을 수 있는" 직선 셀들
            public int Bucket;           // 분산용 버킷 인덱스
        }

        // ===== 튜닝 파라미터 =====
        // 6x6이면 버킷 3x3 추천(각 버킷 2x2 정도)
        private static int ComputeBucketCountAxis(int len)
        {
            if (len <= 6) return 3;
            if (len <= 10) return 4;
            if (len <= 14) return 5;
            return 6;
        }

        private const float BucketWallPower = 2.2f;     // 버킷에 벽이 많을수록 강하게 패널티
        private const float BucketCandPower = 0.35f;    // 후보 수 영향은 약하게(너무 쏠리지 않게)
        private const int DensityRadius = 2;            // 벽 밀집 측정 반경
        private const float DensityPower = 2.2f;        // 밀집 패널티 강도

        // ===== 외부 API =====
        public static MapData CreateMap(int width, int height, int wallCount, Tile playerStart)
        {
            var map = new int[height, width]; // 0 road, 1 wall

            // 기본 유효성
            if (!In(playerStart.X, playerStart.Z, width, height))
                playerStart = new Tile(width / 2, height / 2);

            // 1) 첫 벽: 반드시 "외곽" + "playerStart와 같은 행/열"에 배치해서 첫 이동 성립 보장
            // wallCount가 0이면 아무 것도 안 놓음
            if (wallCount > 0)
            {
                PlaceFirstBorderWallAlignedToStart(map, width, height, playerStart);
            }

            // 버킷 설정
            int sx = ComputeBucketCountAxis(width);
            int sz = ComputeBucketCountAxis(height);
            int bucketTotal = sx * sz;

            // 버킷별 벽 카운트
            var wallsByBucket = new int[bucketTotal];

            // 첫 벽도 버킷 카운트 반영
            CountWallsIntoBuckets(map, width, height, sx, sz, wallsByBucket);

            // 2) 반복: frontier 기반으로 벽 추가
            for (int i = 1; i < wallCount; i++)
            {
                // (A) 플레이어 start에서 도달 가능한 "정지점"만 계산
                var reachableStops = ComputeReachableStops(map, width, height, playerStart);
                if (reachableStops.Count == 0) break;

                // (B) reachableStops에서 frontier 생성
                var frontiers = BuildFrontiers(map, width, height, reachableStops, sx, sz);
                if (frontiers.Count == 0) break;

                // (C) 버킷별 후보(frontier) 분류
                var frontierByBucket = new Dictionary<int, List<int>>(); // bucket -> frontier indices
                for (int fi = 0; fi < frontiers.Count; fi++)
                {
                    int b = frontiers[fi].Bucket;
                    if (!frontierByBucket.TryGetValue(b, out var list))
                    {
                        list = new List<int>();
                        frontierByBucket[b] = list;
                    }
                    list.Add(fi);
                }

                // (D) 버킷 선택 (벽 밀도 패널티 + 후보 수 영향 약하게)
                int pickedBucket = PickBucketWeighted(frontierByBucket, wallsByBucket);

                // (E) 선택된 버킷 내부에서 frontier 하나 선택(랜덤)
                var fidxList = frontierByBucket[pickedBucket];
                var pickedFrontier = frontiers[fidxList[UnityEngine.Random.Range(0, fidxList.Count)]];

                // (F) frontier의 직선 후보 셀 중 "밀집이 낮은" 곳을 가중 랜덤 선택
                Tile wallPos = PickCellSpreadWeighted(map, width, height, pickedFrontier.LineCells, DensityRadius, DensityPower);

                // 혹시 이미 벽이면(이론상 거의 없음) 스킵
                if (IsBlocker(map, wallPos)) { i--; continue; }

                // (G) 벽 설치
                map[wallPos.Z, wallPos.X] = 1;

                // 버킷 벽 카운트 업데이트
                int bw = GetBucket(wallPos.X, wallPos.Z, width, height, sx, sz);
                wallsByBucket[bw]++;
            }

            // 3) MapData로 변환
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

        // ===== 1) 첫 벽: 외곽 + start와 같은 행/열 =====
        private static void PlaceFirstBorderWallAlignedToStart(int[,] map, int w, int h, Tile start)
        {
            // start에서 외곽까지 4방향 중 하나를 랜덤 선택 후,
            // 그 방향의 외곽 셀을 벽으로 만든다.
            // => start에서 해당 방향으로 슬라이드하면 반드시 벽을 만나 "정지"가 생김.

            var options = new List<(Dir dir, Tile borderWall)>(4);

            // Up: z = h-1
            if (start.Z < h - 1) options.Add((Dir.Up, new Tile(start.X, h - 1)));
            // Down: z = 0
            if (start.Z > 0) options.Add((Dir.Down, new Tile(start.X, 0)));
            // Right: x = w-1
            if (start.X < w - 1) options.Add((Dir.Right, new Tile(w - 1, start.Z)));
            // Left: x = 0
            if (start.X > 0) options.Add((Dir.Left, new Tile(0, start.Z)));

            if (options.Count == 0) return;

            var pick = options[UnityEngine.Random.Range(0, options.Count)].borderWall;

            // start가 외곽이면 pick이 start일 수 있으니 방지
            if (pick.X == start.X && pick.Z == start.Z)
            {
                // fallback: 인접한 다른 외곽
                if (start.X != 0) pick = new Tile(0, start.Z);
                else if (start.X != w - 1) pick = new Tile(w - 1, start.Z);
                else if (start.Z != 0) pick = new Tile(start.X, 0);
                else pick = new Tile(start.X, h - 1);
            }

            map[pick.Z, pick.X] = 1;
        }

        // ===== 2) reachableStops: start에서 "슬라이딩 이동"으로 도달 가능한 정지점만 =====
        private static HashSet<int> ComputeReachableStops(int[,] map, int w, int h, Tile start)
        {
            var visited = new HashSet<int>();
            var q = new Queue<Tile>();

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

        /// <summary>
        /// from에서 dir로 슬라이드했을 때
        /// - blocker(벽/적/StopTile)를 "만나서 멈출 수 있으면" stop 반환
        /// - blocker를 못 만나고 맵 밖으로 나가면 invalid
        /// - 첫 칸부터 blocker면 이동 자체가 성립하지 않으니 invalid
        /// </summary>
        private static Tile SlideToStopIfBlocked(int[,] map, int w, int h, Tile from, Dir dir)
        {
            var (dx, dz) = D[(int)dir];

            int lastX = from.X;
            int lastZ = from.Z;

            int nx = lastX + dx;
            int nz = lastZ + dz;

            if (!In(nx, nz, w, h)) return new Tile(int.MinValue, int.MinValue);
            if (map[nz, nx] == 1) return new Tile(int.MinValue, int.MinValue); // 바로 막히면 이동 불가로 처리(네 규칙)

            while (true)
            {
                int tx = lastX + dx;
                int tz = lastZ + dz;

                if (!In(tx, tz, w, h))
                    return new Tile(int.MinValue, int.MinValue); // 끝까지 blocker 못 만남 => 이 방향 이동 성립 X

                if (map[tz, tx] == 1)
                    return new Tile(lastX, lastZ); // blocker 직전이 stop

                lastX = tx;
                lastZ = tz;
            }
        }

        // ===== 3) Frontier 생성: reachableStops에서 "벽이 없는 방향"만 확장 후보 =====
        private static List<Frontier> BuildFrontiers(int[,] map, int w, int h, HashSet<int> reachableStops, int sx, int sz)
        {
            var list = new List<Frontier>();

            foreach (var idx in reachableStops)
            {
                int z = idx / w;
                int x = idx % w;
                var from = new Tile(x, z);

                // from 자체가 벽이면 제외(원칙상 없음)
                if (IsBlocker(map, from)) continue;

                for (int di = 0; di < 4; di++)
                {
                    var dir = (Dir)di;

                    // 이미 이 방향으로 blocker를 만날 수 있으면 "이미 이동 가능"이므로 frontier가 아님
                    if (HasBlockerAhead(map, w, h, from, dir))
                        continue;

                    // blocker가 없는 방향: 직선 위 어디엔가 벽을 놓으면 새 stop이 생긴다
                    var line = CollectLineCells(map, w, h, from, dir);
                    if (line.Count == 0) continue;

                    // 버킷은 "from stop 기준"으로 두는 게 보통 안정적
                    int bucket = GetBucket(from.X, from.Z, w, h, sx, sz);

                    list.Add(new Frontier
                    {
                        FromStop = from,
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

        private static List<Tile> CollectLineCells(int[,] map, int w, int h, Tile from, Dir dir)
        {
            var (dx, dz) = D[(int)dir];
            var line = new List<Tile>();

            int x = from.X;
            int z = from.Z;

            while (true)
            {
                x += dx; z += dz;
                if (!In(x, z, w, h)) break;
                if (map[z, x] == 1) break;

                line.Add(new Tile(x, z));
            }

            return line;
        }

        // ===== 4) 버킷 선택: 벽 밀도 패널티 + 후보 수 영향 약하게 =====
        private static int PickBucketWeighted(Dictionary<int, List<int>> frontierByBucket, int[] wallsByBucket)
        {
            float total = 0f;
            var keys = new List<int>(frontierByBucket.Count);
            var weights = new List<float>(frontierByBucket.Count);

            foreach (var kv in frontierByBucket)
            {
                int bucket = kv.Key;
                int cand = kv.Value.Count;
                if (cand <= 0) continue;

                int walls = wallsByBucket[bucket];

                // 후보 수는 약하게 영향, 벽 수는 강하게 패널티
                float wCand = Mathf.Pow(cand + 1f, BucketCandPower);
                float wWall = 1f / Mathf.Pow(walls + 1f, BucketWallPower);

                float score = wCand * wWall;

                keys.Add(bucket);
                weights.Add(score);
                total += score;
            }

            // fallback
            if (keys.Count == 0) return 0;

            float r = UnityEngine.Random.Range(0f, total);
            float acc = 0f;
            for (int i = 0; i < keys.Count; i++)
            {
                acc += weights[i];
                if (r <= acc) return keys[i];
            }
            return keys[keys.Count - 1];
        }

        // ===== 5) 버킷 내부/라인 내부 선택: 밀집 낮은 곳 우선 =====
        private static Tile PickCellSpreadWeighted(int[,] map, int w, int h, List<Tile> cells, int radius, float power)
        {
            if (cells == null || cells.Count == 0)
                return new Tile(int.MinValue, int.MinValue);

            float total = 0f;
            var weights = new float[cells.Count];

            for (int i = 0; i < cells.Count; i++)
            {
                var t = cells[i];
                if (IsBlocker(map, t))
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

        // ===== 6) 버킷/유틸 =====
        private static int GetBucket(int x, int z, int w, int h, int sx, int sz)
        {
            int bx = Mathf.Clamp((x * sx) / w, 0, sx - 1);
            int bz = Mathf.Clamp((z * sz) / h, 0, sz - 1);
            return bz * sx + bx;
        }

        private static void CountWallsIntoBuckets(int[,] map, int w, int h, int sx, int sz, int[] wallsByBucket)
        {
            Array.Clear(wallsByBucket, 0, wallsByBucket.Length);
            for (int z = 0; z < h; z++)
                for (int x = 0; x < w; x++)
                {
                    if (map[z, x] != 1) continue;
                    int b = GetBucket(x, z, w, h, sx, sz);
                    wallsByBucket[b]++;
                }
        }

        private static bool IsBlocker(int[,] map, Tile t) => map[t.Z, t.X] == 1;

        private static bool In(int x, int z, int w, int h)
            => x >= 0 && x < w && z >= 0 && z < h;
    }
}
