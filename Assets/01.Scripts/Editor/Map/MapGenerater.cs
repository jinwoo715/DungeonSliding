using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MapGenerater : MonoBehaviour
    {
        public enum EMoveDirection
        {
            Up,
            Right,
            Left,
            Down
        }

        public enum ESectionType
        {
            LeftUp,
            RightUp,
            LeftDown,
            RightDown
        }

        public static Dictionary<ESectionType, int> wallCountDic;
        public static List<Tile> StopPoints;

        //처음 StopTile 생성.
        //끝 점인지 확인, 방향 설정.
        //방향에 따라 다음 StopTile목록 찾기.
        //섹션에서 더 적은 쪽 가중치 후 랜덤 StopTile생성
        //반복

        public static MapData CreateMap(int width, int height, int wallCount)
        {
            int[] tiles = new int[width * height];
            for (int i = 0; i < tiles.Length; i++) tiles[i] = 0;

            int RanStartP = UnityEngine.Random.Range(0, 4);


            EMoveDirection dir = (EMoveDirection)RanStartP;
            
            int ranW = UnityEngine.Random.Range(0, width);
            int ranH = UnityEngine.Random.Range(0, height);

            Tile stopTile = new Tile();

            switch (dir)
            {
                case EMoveDirection.Up:
                    stopTile.Z = height - 1;
                    stopTile.X = ranW;
                    break;
                case EMoveDirection.Right:
                    stopTile.X = width-1;
                    stopTile.Z = ranH;
                    break;
                case EMoveDirection.Left:
                    stopTile.X = 0;
                    stopTile.Z = ranH;
                    break;
                case EMoveDirection.Down:
                    stopTile.Z = 0;
                    stopTile.X = ranW;
                    break;
            }

            StopPoints.Add(stopTile);
            ESectionType section = GetSection(stopTile, width, height);

            if (!wallCountDic.ContainsKey(section))
                wallCountDic.Add(section, 0);

            wallCountDic[section]++;


            EMoveDirection toDir;
            switch (dir)
            {
                case EMoveDirection.Up:

                    if (stopTile.X == 0)
                        toDir = EMoveDirection.Left;
                    else if (stopTile.X == width - 1)
                        toDir = EMoveDirection.Right;
                    else
                    {
                        int ran = UnityEngine.Random.Range(0, 2);

                        if (ran == 0) toDir = EMoveDirection.Left;
                        else toDir = EMoveDirection.Right;
                    }

                    break;
                case EMoveDirection.Right:
                    if (stopTile.X == 0)
                        toDir = EMoveDirection.Left;
                    else if (stopTile.X == width - 1)
                        toDir = EMoveDirection.Right;
                    else
                    {
                        int ran = UnityEngine.Random.Range(0, 2);

                        if (ran == 0) toDir = EMoveDirection.Left;
                        else toDir = EMoveDirection.Right;
                    }
                    break;
                case EMoveDirection.Left:
                    if (stopTile.X == 0)
                        toDir = EMoveDirection.Left;
                    else if (stopTile.X == width - 1)
                        toDir = EMoveDirection.Right;
                    else
                    {
                        int ran = UnityEngine.Random.Range(0, 2);

                        if (ran == 0) toDir = EMoveDirection.Left;
                        else toDir = EMoveDirection.Right;
                    }

                    break;
                case EMoveDirection.Down:
                    if (stopTile.X == 0)
                        toDir = EMoveDirection.Left;
                    else if (stopTile.X == width - 1)
                        toDir = EMoveDirection.Right;
                    else
                    {
                        int ran = UnityEngine.Random.Range(0, 2);

                        if (ran == 0) toDir = EMoveDirection.Left;
                        else toDir = EMoveDirection.Right;
                    }
                    break;
            }


            var data = ScriptableObject.CreateInstance<MapData>();
            data.Width = width;
            data.Height = height;
            data.MapTiles = tiles;

            return data;
        }

        private static bool IsInArea(Tile tile, int width, int height)
        {
            return !(tile.X < 0 || tile.X >= width || tile.Z < 0 || tile.Z >= height);
        }

        private static bool IsAttackOuter(Tile tile, int width, int height)
        {
            return tile.X == 0 || tile.X == width - 1 || tile.Z == 0 || tile.Z == height - 1;
        }

        private static ESectionType GetSection(Tile tile, int width, int height)
        {
            if (tile.Z >= height / 2)
            {
                //UpRight
                if (tile.X >= width / 2) return ESectionType.RightUp;
                //UpLeft
                else return ESectionType.LeftUp;
            }
            else
            {
                //DownRight
                if (tile.X >= width / 2) return ESectionType.RightDown;
                //DownLeft
                else return ESectionType.LeftDown;
            }
        }
    }
}
