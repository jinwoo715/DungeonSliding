#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace JW.DungeonSliding
{
    public struct MapDataContext
    {
        public readonly string MapName;
        public readonly int[] TileArray;
        public readonly int XCount;
        public readonly int ZCount;

        public readonly IReadOnlyDictionary<Tile, EffectObjectData> EffectObjData;
        public readonly List<CreatureTemplete> CreatureTempletes;

        public MapDataContext
            (string name, int[] tiles, int xCount, int zCount, 
            IReadOnlyDictionary<Tile, EffectObjectData> effectData, List<CreatureTemplete> creatureTempletes
            )
        {
            MapName = name;
            TileArray = tiles;
            XCount = xCount;
            ZCount = zCount;
            EffectObjData = effectData;
            CreatureTempletes = creatureTempletes;
        }
    }

    public class MapEditState
    {
        public string MapName;

        public int xCount;
        public int zCount;

        private float _windowWidth;
        private float _windowHeight;

        public float _gridFieldWidth;
        public float _dataFieldWidth;

        private float _gridFieldX;

        private int[] _tileMap;
        private Dictionary<Tile, EffectObjectData> _effectObjData = new Dictionary<Tile, EffectObjectData>();
        private List<CreatureTemplete> _creatureTempletes = new List<CreatureTemplete>();

        public float WindowHeight => _windowHeight;
        public float GridFieldX => _gridFieldX;

        public IReadOnlyDictionary<Tile, EffectObjectData> EffectObjects => _effectObjData;

        public MapEditState()
        {
            AddEnemyTemplete();
        }
        public void UpdateFieldArea(float windowWidth, float windowHeight)
        {
            _windowWidth = windowWidth;
            _windowHeight = windowHeight;

            _dataFieldWidth = 250;

            _gridFieldX = _dataFieldWidth + 50;

            _gridFieldWidth = _windowWidth - _gridFieldX - 20;
        }

        //Tile
        public void InitTileMap(int x, int z)
        {
            xCount = x;
            zCount = z;
            _tileMap = new int[zCount * xCount];
        }
        public void SetTileType(Tile point)
        {
            int value = (_tileMap[(xCount * point.Z) + point.X] + 1) % 2;

            if((ETileType)value == ETileType.Wall)
            {
                if (IsSettedCreatureTile(point) || IsSettedEffectTile(point)) return;
            }

            _tileMap[(xCount * point.Z) + point.X] = value;
        }

        public bool IsSettedCreatureTile(Tile point)
        {
            for (int i = 0; i < _creatureTempletes.Count; i++)
            {
                CreatureTemplete templete = _creatureTempletes[i];

                if (templete.PlayerPos == point) return true;

                for (int j = 0; j < templete.NomalEnemyPos.Count; j++)
                {
                    if (templete.NomalEnemyPos[j] == point) return true;
                }
                for (int j = 0; j < templete.BossEnemyPos.Count; j++)
                {
                    if (templete.BossEnemyPos[j] == point) return true;
                }
            }

            return false;
        }
        private bool IsSettedEffectTile(Tile point)
        {
            return _effectObjData.ContainsKey(point);
        }


        public int GetTileType(int x, int z)
        {
            if (_tileMap == null ||_tileMap.Length == 0) return 0;
            if (!IsBounds(x, z)) return 0;

            return _tileMap[ToIndex(x, z)];
        }
        public bool IsRoute(Tile point)
        {
            if (_tileMap == null) return false;
            if (!IsBounds(point)) return false;

            return _tileMap[ToIndex(point)] == (int)ETileType.Route;
        }

        public bool IsEffectTile(Tile point) => _effectObjData.ContainsKey(point);

        private bool IsBounds(Tile p)
        {
            return p.X >= 0 && p.Z >= 0 && p.X < xCount && p.Z < zCount;
        }
        private bool IsBounds(int x, int z)
        {
            return x >= 0 && z >= 0 && x < xCount && z < zCount;
        }
        private int ToIndex(Tile p)
        {
            return (xCount * p.Z) + p.X;
        }
        private int ToIndex(int x, int z)
        {
            return (xCount * z) + x;
        }

        public int AddEnemyTemplete()
        {
            _creatureTempletes.Add(new CreatureTemplete());

            return _creatureTempletes.Count - 1;
        }
        public int RemoveEnemyTemplete(int index)
        {
            if (_creatureTempletes.Count == 1)
                return 0;

            _creatureTempletes.RemoveAt(index);
            return _creatureTempletes.Count - 1 > index ? index : _creatureTempletes.Count - 1;
        }
        public int GetEnemyTempleteCount()
        {
            return _creatureTempletes.Count;
        }

        //EffectObject
        public void RemoveEffectObject(Tile point)
        {
            _effectObjData.Remove(point);
        }
        public void SetEffectObj(EffectObjectData effectTileData)
        {
            _effectObjData[effectTileData.Point] = effectTileData;
        }

        public bool TryGetEffectObject(Tile point, out EffectObjectData effectTileData)
        {
            if(_effectObjData.TryGetValue(point, out EffectObjectData data))
            {
                effectTileData = data;
                return true;
            }
            else
            {
                effectTileData = default;
                return false;
            }
        }
        public int GetTeleports(out Tile t1, out Tile t2)
        {
            t1 = Tile.Invalid;
            t2 = Tile.Invalid;

            int teleportCount = 0;

            foreach (var effectObj in _effectObjData)
            {
                if (effectObj.Value.EffectObjectType != EEffectObjectType.Teleport)
                    continue;

                if (!t1.IsValid) t1 = effectObj.Value.Point;
                else t2 = effectObj.Value.Point;
                
                teleportCount++;

                if (teleportCount >= 2)
                    break;
            }

            return teleportCount;
        }

        public MapDataContext GetMapDataContext()
        {
            MapDataContext mapDataContext = new MapDataContext(MapName, _tileMap, xCount, zCount, _effectObjData, _creatureTempletes);
            return mapDataContext;
        }

        public void LoadFromMapData(MapData data)
        {
            MapName = data.name; // 또는 data 파일명 기반으로
            InitTileMap(data.Width, data.Height);

            // 타일 복사
            _tileMap = (int[])data.MapTiles.Clone(); // 내부 필드 접근 가능하게 하거나 SetTileMap 메서드 추가


            // effect
            _effectObjData.Clear();

            if (data.effectTileDatas == null) return;
            foreach (var e in data.effectTileDatas)
                _effectObjData[e.Point] = e;
        }

        internal CreatureTemplete GetCretureTemplete(int templeteNum)
        {
            return _creatureTempletes[templeteNum];
        }
    }
}
#endif