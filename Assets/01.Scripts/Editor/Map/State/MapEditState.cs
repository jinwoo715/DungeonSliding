#if UNITY_EDITOR

using System.Collections.Generic;

namespace JW.DungeonSliding
{
    public struct MapDataContext
    {
        public readonly string MapName;
        public readonly int[] TileArray;
        public readonly int XCount;
        public readonly int ZCount;
        public readonly Tile PlayerPoint;

        public readonly IReadOnlyDictionary<Tile, EffectObjectData> EffectObjData;
        public readonly IReadOnlyList<EnemyTempleteSheet> EnemyTempleteSheet;

        public MapDataContext
            (string name, int[] tiles, int xCount, int zCount, Tile playerPoint,
            IReadOnlyDictionary<Tile, EffectObjectData> effectData, IReadOnlyList<EnemyTempleteSheet> enemyTemplete
            )
        {
            MapName = name;
            TileArray = tiles;
            XCount = xCount;
            ZCount = zCount;
            PlayerPoint = playerPoint;
            EffectObjData = effectData;
            EnemyTempleteSheet = enemyTemplete;
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
        private Tile _playerPoint = new Tile(-1,-1);
        private List<EnemyTempleteSheet> _enemyTempleteSheet = new List<EnemyTempleteSheet>();
        private Dictionary<Tile, EffectObjectData> _effectObjData = new Dictionary<Tile, EffectObjectData>();

        public float WindowHeight => _windowHeight;
        public float GridFieldX => _gridFieldX;

        public Tile PlayerPoint => _playerPoint;
        public IReadOnlyDictionary<Tile, EffectObjectData> EffectObjects => _effectObjData;


        public MapEditState()
        {
            _enemyTempleteSheet.Add(new EnemyTempleteSheet());
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
        public void SetTileType(Tile point, ETileType tileType)
        {
            _tileMap[(xCount * point.ZPos) + point.XPos] = (int)tileType;
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

        private bool IsBounds(Tile p)
        {
            return p.XPos >= 0 && p.ZPos >= 0 && p.XPos < xCount && p.ZPos < zCount;
        }
        private bool IsBounds(int x, int z)
        {
            return x >= 0 && z >= 0 && x < xCount && z < zCount;
        }
        private int ToIndex(Tile p)
        {
            return (xCount * p.ZPos) + p.XPos;
        }
        private int ToIndex(int x, int z)
        {
            return (xCount * z) + x;
        }
        
        public bool IsExistEnemy(Tile point) 
        {
            for (int i = 0; i < _enemyTempleteSheet.Count; i++)
            {
                if (_enemyTempleteSheet[i].EnemyData.ContainsKey(point))
                {
                    return true;
                }
            }

            return false;
        }
        public EnemyTempleteSheet GetEnemyTemplete(int templeteNum)
        {
            if (templeteNum < 0 || _enemyTempleteSheet.Count <= templeteNum)
                return null;

            return _enemyTempleteSheet[templeteNum];
        }
        public EnemySettingData GetEnemy(int templeteNum, Tile point)
        {
            if (_enemyTempleteSheet[templeteNum].EnemyData.TryGetValue(point, out EnemySettingData value))
            {
                return value;
            }
            else
                return null;
        }
        public void SetEnemy(int templeteNum, Tile point, string enemyUid)
        {
            if(_enemyTempleteSheet[templeteNum].EnemyData.TryGetValue(point, out EnemySettingData data))
            {
                data.EnemyUID = enemyUid;
                data.Point = point;
            }
            else
            {
                _enemyTempleteSheet[templeteNum].EnemyData.Add(point, new EnemySettingData(enemyUid,point));
            }
        }
        public void RemoveEnemy(int templeteNum, Tile point)
        {
            _enemyTempleteSheet[templeteNum].EnemyData.Remove(point);
        }
        public int AddEnemyTemplete()
        {
            _enemyTempleteSheet.Add(new EnemyTempleteSheet());
            return _enemyTempleteSheet.Count - 1;
        }
        public int RemoveEnemyTemplete(int index)
        {
            if (_enemyTempleteSheet.Count == 1)
                return 0;

            _enemyTempleteSheet.RemoveAt(index);
            return _enemyTempleteSheet.Count - 1 > index ? index : _enemyTempleteSheet.Count - 1;
        }
        public int GetEnemyTempleteCount()
        {
            return _enemyTempleteSheet.Count;
        }

        //Player
        public void SetPlayerPoint(Tile point)
        {
            _playerPoint = point;
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
            MapDataContext mapDataContext = new MapDataContext(MapName, _tileMap, xCount, zCount, _playerPoint, _effectObjData, _enemyTempleteSheet);
            return mapDataContext;
        }

        public void LoadFromMapData(MapData data)
        {
            MapName = data.name; // 또는 data 파일명 기반으로
            InitTileMap(data.Width, data.Height);

            // 타일 복사
            _tileMap = (int[])data.MapTiles.Clone(); // 내부 필드 접근 가능하게 하거나 SetTileMap 메서드 추가

            // 플레이어
            _playerPoint = data.PlayerPosition;

            // enemy template
            _enemyTempleteSheet.Clear();
            for (int i = 0; i < data.EnemyTemplete.Length; i++)
            {
                var sheet = new EnemyTempleteSheet();
                foreach (var enemy in data.EnemyTemplete[i].EnemyData)
                    sheet.EnemyData[enemy.Point] = enemy;

                _enemyTempleteSheet.Add(sheet);
            }

            // effect
            _effectObjData.Clear();
            foreach (var e in data.effectTileDatas)
                _effectObjData[e.Point] = e;
        }
    }
}
#endif