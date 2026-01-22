using System;
using System.Collections.Generic;
using UnityEngine;
namespace JW.SlidingPuzzle {
    public class MapManager : MonoBehaviour, IBoard
    {
        [SerializeField] private TileGenerator _tileMap;
        [SerializeField] private EffectObjectGenerator _effectObjectGenerator;

        public Action<EnemyTemplete[]> SetEnemyEvent;
        
        private bool[] _tileMapData;
        private HashSet<TilePoint> _enemies = new HashSet<TilePoint>();
        private Dictionary<TilePoint, IEffectObject> _effectTileDic = new Dictionary<TilePoint, IEffectObject>();
        private MapData _currentMapData;

        private int[,] _dir = { {-1,0 }, {0,1 }, {1,0 }, {0,-1 } };

        public void Init()
        {
            _tileMap.Init(this);
            _effectObjectGenerator.SetBoard(this);
        }

        public void SetMap(MapData mapData, ICombatant player)
        {
            _currentMapData = mapData;

            _tileMapData = new bool[mapData.Height * mapData.Width];

            _tileMap.SetMap(mapData.MapTiles, mapData.Height, mapData.Width);
            _effectObjectGenerator.SetMap(mapData.effectTileDatas);

            SetEnemyEvent?.Invoke(mapData.EnemyTemplete);
            player.SetPosition(mapData.PlayerPosition);
        }
        
        public MoveContext GetMoveContext(TilePoint startPoint, EDirectionType direction, ETileEnterType enterType)
        {
            MoveContext moveContext = new MoveContext(startPoint, direction, enterType);

            if(_effectTileDic.TryGetValue(startPoint, out IEffectObject effectTile))
            {
                effectTile.OnEnterTile(ref moveContext);

                ESlideResultType slideResultType = moveContext.ResultType;

                if (slideResultType == ESlideResultType.Stop || slideResultType == ESlideResultType.Teleport)
                {
                    return moveContext;
                }
            }

            TilePoint destination = startPoint;

            destination.XPos += _dir[(int)moveContext.Direction, 0];
            destination.ZPos += _dir[(int)moveContext.Direction, 1];

            if (!IsInArea(destination) || _tileMapData[GetTileIndex(destination)] == false)
            {
                moveContext.ResultType = ESlideResultType.Stop;
                return moveContext;
            }

            if (_enemies.Contains(destination))
            {
                moveContext.ResultType = ESlideResultType.EnemyStop;
                return moveContext;
            }

            moveContext.DestTile = destination;
            moveContext.EnterType = ETileEnterType.Slide;

            return moveContext;
        }

        private bool IsInArea(TilePoint data)
        {
            if (data.XPos < 0 || data.XPos >= _currentMapData.Width || data.ZPos < 0 || data.ZPos >= _currentMapData.Height)
            {
                return false;
            }
            else
                return true;
        }

        //interface
        public void RegisterTileBoard(TilePoint point, bool isWalkable)
        {
            _tileMapData[GetTileIndex(point.XPos, point.ZPos)] = isWalkable;
        }
        private int GetTileIndex(int x, int z) => _currentMapData.Width * z + x;
        private int GetTileIndex(TilePoint point) => _currentMapData.Width * point.ZPos + point.XPos;
        public void RegisterEnemyBoard(TilePoint point, ICombatant combatant)
        {
            _enemies.Add(point);
        }
        public void UnRegisterEnemyBoard(TilePoint point)
        {
            _enemies.Remove(point);
        }
        public void ClearEnemyBoard()
        {
            _enemies.Clear();
        }

        public void RegisterEffectObject(TilePoint point, IEffectObject effectObj)
        {
            _effectTileDic.Add(point, effectObj);
        }

        public void UnRegisterEffectObject(TilePoint point)
        {
            _effectTileDic.Remove(point);
        }
    }
}
