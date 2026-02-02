using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Combat;
using JW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace JW.DungeonSliding.Map 
{
    public class MapManager : MonoBehaviour, IBoard
    {
        [SerializeField] private TileGenerator _tileMap;
        [SerializeField] private EffectObjectGenerator _effectObjectGenerator;

        private ShuffleBag<MapData> _mapBag;

        public Action<EnemyTemplete[], int> SetEnemyEvent;
        
        private bool[] _tileMapData;
        private HashSet<Tile> _enemyTiles = new HashSet<Tile>();
        private HashSet<Tile> _obstacleTiles = new HashSet<Tile>();
        private Dictionary<Tile, IEffectTile> _effectTileDic = new Dictionary<Tile, IEffectTile>();
        private MapData _currentMapData;

        private int[,] _dir = { { 0,1 }, {1,0 }, {0,-1 }, { -1, 0 } };
        private ITilePosition _player;

        public void Init(ITilePosition player)
        {
            _player = player;
            _mapBag = new ShuffleBag<MapData>(GameManager.Instance.Resource.MapData);
            _tileMap.Init(this);
            _effectObjectGenerator.SetBoard(this);
        }

        public void SetMap(int floor)
        {
            _currentMapData = _mapBag.GetItem();

            _tileMapData = new bool[_currentMapData.Height * _currentMapData.Width];

            _tileMap.SetMap(_currentMapData.MapTiles, _currentMapData.Height, _currentMapData.Width);
            _effectObjectGenerator.SetMap(_currentMapData.effectTileDatas);

            SetEnemyEvent?.Invoke(_currentMapData.EnemyTemplete, floor);
            _player.SetPosition(_currentMapData.PlayerPosition);
        }
        public MoveContext GetMoveContext(Tile startPoint, EDirectionType direction, ETileEnterType enterType)
        {
            MoveContext moveContext = new MoveContext(startPoint, direction, enterType);

            if(_effectTileDic.TryGetValue(startPoint, out IEffectTile effectTile))
            {
                effectTile.OnEnterTile(ref moveContext);

                ESlideResultType slideResultType = moveContext.ResultType;

                if (slideResultType == ESlideResultType.Stop || slideResultType == ESlideResultType.Teleport)
                {
                    return moveContext;
                }
            }

            Tile destination = startPoint;

            destination.XPos += _dir[(int)moveContext.Direction, 0];
            destination.ZPos += _dir[(int)moveContext.Direction, 1];

            if (!IsInArea(destination) || _tileMapData[GetTileIndex(destination)] == false || _obstacleTiles.Contains(destination))
            {
                moveContext.ResultType = ESlideResultType.Stop;
                return moveContext;
            }

            if (_enemyTiles.Contains(destination))
            {
                moveContext.ResultType = ESlideResultType.EnemyStop;
                return moveContext;
            }

            moveContext.DestTile = destination;
            moveContext.EnterType = ETileEnterType.Slide;

            return moveContext;
        }
        private bool IsInArea(Tile data)
        {
            if (data.XPos < 0 || data.XPos >= _currentMapData.Width || data.ZPos < 0 || data.ZPos >= _currentMapData.Height)
            {
                return false;
            }
            else
                return true;
        }

        //interface
        public void RegisterTileBoard(Tile point, bool isWalkable)
        {
            _tileMapData[GetTileIndex(point.XPos, point.ZPos)] = isWalkable;
        }
        private int GetTileIndex(int x, int z) => _currentMapData.Width * z + x;
        private int GetTileIndex(Tile point) => _currentMapData.Width * point.ZPos + point.XPos;
        public void RegisterEnemyTile(Tile point)
        {
            _enemyTiles.Add(point);
        }
        public void UnRegisterEnemyTile(Tile point)
        {
            _enemyTiles.Remove(point);
        }
        public void ClearEnemyBoard()
        {
            _enemyTiles.Clear();
        }
        public void RegisterEffectObject(Tile point, IEffectTile effectObj)
        {
            _effectTileDic.Add(point, effectObj);
        }
        public void UnRegisterEffectObject(Tile point)
        {
            _effectTileDic.Remove(point);
        }

        public void RegisterObstacleTile(Tile point)
        {
            _obstacleTiles.Add(point);
        }

        public void UnRegisterObstacleTile(Tile point)
        {
            _obstacleTiles.Remove(point);
        }
    }
}
