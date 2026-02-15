using System;
using System.Collections.Generic;
using UnityEngine;

public static class SlideMapGen
{
    public enum Dir { Up, Right, Down, Left }
    static readonly (int dx, int dz)[] D =
    {
        (0, 1), (1, 0), (0, -1), (-1, 0)
    };

    public struct P { public int x, z; public P(int x, int z) { this.x = x; this.z = z; } }
    public struct Frontier
    {
        public P from;
        public Dir dir;
        public List<P> lineCells; // 이 직선 위에 "벽을 놓을 수 있는 칸들"
        public int section;       // 섹션 분산용(원하면)
    }

    // 0=길, 1=벽(적/StopTile도 여기로 합쳐도 됨)
    public static int[] Generate(int w, int h, int wallCount, P start)
    {
        var map = new int[h, w];

        var stops = new HashSet<int>();
        stops.Add(start.z * w + start.x);

        // 1) 첫 벽: 외곽에 생성(스타트에서 최소 1방향 벽 만나게 보장하는 방식 추천)
        PlaceFirstBorderWall(map, w, h, start);

        // 2) 반복 배치
        for (int i = 1; i < wallCount; i++)
        {
            var frontiers = BuildFrontiers(map, w, h, stops);

            if (frontiers.Count == 0) break;

            // 섹션 밸런스/밀집 방지 같은건 여기서 "frontier 선택"과 "cell 선택"에 점수로 넣으면 됨
            var f = frontiers[UnityEngine.Random.Range(0, frontiers.Count)];

            // 직선 위 후보 중 하나를 골라 벽 설치 (밀집 방지: 주변 벽 적은 칸 우선)
            var wallPos = PickCellSpreadWeighted(map, w, h, f.lineCells, radius: 2, power: 2.2f);

            map[wallPos.z, wallPos.x] = 1;

            // newStop = from에서 dir로 슬라이드하면 방금 벽 직전에서 멈춤
            var newStop = SlideToStop(map, w, h, f.from, f.dir);

            if (newStop.x != int.MinValue)
                stops.Add(newStop.z * w + newStop.x);
        }

        int[] mapDatas = new int[h * w];

        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                mapDatas[i * w + j] = map[i,j];
            }
        }
        return mapDatas;
    }

    // --- Frontier 만들기: "현재 stops에서 벽을 못 만나는 방향"을 확장 후보로 만든다 ---
    static List<Frontier> BuildFrontiers(int[,] map, int w, int h, HashSet<int> stops)
    {
        var list = new List<Frontier>();

        foreach (var idx in stops)
        {
            int z = idx / w;
            int x = idx % w;
            var from = new P(x, z);

            for (int di = 0; di < 4; di++)
            {
                var dir = (Dir)di;

                // 이 방향으로 "이미 벽을 만날 수 있으면" 이건 frontier 아님(이미 이동 가능)
                if (HasWallAhead(map, w, h, from, dir))
                    continue;

                // 벽을 못 만나면: 이 직선 위 어디엔가 벽을 놓을 수 있음(후보 라인 생성)
                var line = CollectLineCells(map, w, h, from, dir);
                if (line.Count == 0) continue;

                list.Add(new Frontier
                {
                    from = from,
                    dir = dir,
                    lineCells = line,
                    section = GetSection(from, w, h)
                });
            }
        }

        return list;
    }

    static bool HasWallAhead(int[,] map, int w, int h, P from, Dir dir)
    {
        var (dx, dz) = D[(int)dir];
        int x = from.x, z = from.z;

        while (true)
        {
            x += dx; z += dz;
            if (!In(x, z, w, h)) return false;     // 끝까지 가도 벽 없음
            if (map[z, x] == 1) return true;       // 벽 만남
        }
    }

    // from 바로 다음칸부터 끝까지, "벽을 세울 수 있는 후보"만 수집
    static List<P> CollectLineCells(int[,] map, int w, int h, P from, Dir dir)
    {
        var (dx, dz) = D[(int)dir];
        var line = new List<P>();

        int x = from.x, z = from.z;
        while (true)
        {
            x += dx; z += dz;
            if (!In(x, z, w, h)) break;
            if (map[z, x] == 1) break;

            // 여기에 룰 추가 가능:
            // - startPos에는 벽 금지
            // - 외곽에만/내부만 등
            line.Add(new P(x, z));
        }
        return line;
    }

    static P SlideToStop(int[,] map, int w, int h, P from, Dir dir)
    {
        var (dx, dz) = D[(int)dir];
        int x = from.x, z = from.z;

        // 다음 칸이 밖이거나 벽이면 이동 자체가 성립 안 함
        int nx = x + dx, nz = z + dz;
        if (!In(nx, nz, w, h)) return new P(int.MinValue, int.MinValue);
        if (map[nz, nx] == 1) return new P(int.MinValue, int.MinValue);

        int lastX = x, lastZ = z;

        while (true)
        {
            int tx = lastX + dx;
            int tz = lastZ + dz;

            if (!In(tx, tz, w, h))
                return new P(int.MinValue, int.MinValue); // 벽을 못 만나면 "정지" 자체가 없음(네 규칙)

            if (map[tz, tx] == 1)
                return new P(lastX, lastZ); // 벽 직전이 stop

            lastX = tx; lastZ = tz;
        }
    }

    // --- 첫 벽: 외곽에 하나 배치(스타트에서 최소 1방향 벽 만나게) ---
    static void PlaceFirstBorderWall(int[,] map, int w, int h, P start)
    {
        // 간단 버전: 외곽 랜덤
        // 더 좋은 버전: start에서 가장 가까운 외곽 라인 중 하나를 골라 벽을 세워 "첫 이동" 보장
        var border = new List<P>();
        for (int x = 0; x < w; x++) { border.Add(new P(x, 0)); border.Add(new P(x, h - 1)); }
        for (int z = 1; z < h - 1; z++) { border.Add(new P(0, z)); border.Add(new P(w - 1, z)); }

        // start에 바로 옆 외곽 벽이 생겨 이동 막히는거 싫으면 여기서 필터링
        var p = border[UnityEngine.Random.Range(0, border.Count)];
        map[p.z, p.x] = 1;
    }

    static P PickCellSpreadWeighted(int[,] map, int w, int h, List<P> cells, int radius, float power)
    {
        float total = 0f;
        var weights = new float[cells.Count];

        for (int i = 0; i < cells.Count; i++)
        {
            int dens = CountWalls(map, w, h, cells[i], radius);
            float wgt = 1f / Mathf.Pow(1f + dens, power);
            if (wgt < 0.0001f) wgt = 0.0001f;
            weights[i] = wgt;
            total += wgt;
        }

        float r = UnityEngine.Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < cells.Count; i++)
        {
            acc += weights[i];
            if (r <= acc) return cells[i];
        }
        return cells[cells.Count - 1];
    }

    static int CountWalls(int[,] map, int w, int h, P c, int r)
    {
        int cnt = 0;
        int minX = Mathf.Max(0, c.x - r), maxX = Mathf.Min(w - 1, c.x + r);
        int minZ = Mathf.Max(0, c.z - r), maxZ = Mathf.Min(h - 1, c.z + r);

        for (int z = minZ; z <= maxZ; z++)
            for (int x = minX; x <= maxX; x++)
                if (!(x == c.x && z == c.z) && map[z, x] == 1) cnt++;

        return cnt;
    }

    static int GetSection(P p, int w, int h)
    {
        bool up = p.z >= h / 2;
        bool right = p.x >= w / 2;
        // 0:LU 1:RU 2:LD 3:RD
        if (up) return right ? 1 : 0;
        return right ? 3 : 2;
    }

    static bool In(int x, int z, int w, int h) => x >= 0 && x < w && z >= 0 && z < h;
}
