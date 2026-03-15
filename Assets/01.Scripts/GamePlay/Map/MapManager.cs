using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Combat;
using JW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace JW.DungeonSliding.Map 
{
    public class MapManager : MonoBehaviour, IBoard, IMoveContextProvider, ITileCheckService
    {
        [SerializeField] private TileGenerator _tileMap;
        [SerializeField] private EffectObjectGenerator _effectObjectGenerator;

        [Header("Outer Wall")]
        [SerializeField] private Transform _upperWall;
        [SerializeField] private Transform _leftWall;
        [SerializeField] private Transform _rightWall;

        [Header("Camera Controller")]
        [SerializeField] private CameraController cameraController;

        public Action<List<Tile>,List<Tile>, int, int> RequestSpawnEnemyEvent;
        
        private bool[] _tileMapData;
        private HashSet<Tile> _enemyTiles = new HashSet<Tile>();
        private HashSet<Tile> _obstacleTiles = new HashSet<Tile>();
        private Dictionary<Tile, IEffectTile> _effectTileDic = new Dictionary<Tile, IEffectTile>();

        private MapBundle MapData;
        private ShuffleBag<MapData> _mapBag;
        private MapData _currentMapData;
        private ShuffleBag<CreatureTemplete> _creatureShuffleBag;

        private int[,] _dir = { { 0,1 }, {1,0 }, {0,-1 }, { -1, 0 } };
        private ITileObject _player;

        int _currentAct = 0;

        public void Init(ITileObject player)
        {
            _player = player;
            MapData = GameManager.Resource.MapBundle;

            _mapBag = new ShuffleBag<MapData>(MapData.GetActMapBundle(_currentAct).MapDatas);

            _tileMap.Init(this);
            _effectObjectGenerator.SetBoard(this);
        }
        public void SetMap(int act, int floor)
        {
            _currentMapData = _mapBag.GetItem();
            _creatureShuffleBag = new ShuffleBag<CreatureTemplete>(_currentMapData.CretureTempletes);

            _tileMapData = new bool[_currentMapData.Height * _currentMapData.Width];

            _tileMap.SetMap(_currentMapData.MapTiles, _currentMapData.Height, _currentMapData.Width);
            _effectObjectGenerator.SetMap(_currentMapData.effectTileDatas);

            var templete = _creatureShuffleBag.GetItem();

            int actNum = floor % 3;

            RequestSpawnEnemyEvent?.Invoke(templete.NomalEnemyPos, templete.BossEnemyPos, act, floor);

            _player.SetPosition(templete.PlayerPos);

            float x = (_currentMapData.Height / 2) - 0.5f;

            _leftWall.transform.localScale = new Vector3(1, 10, _currentMapData.Height);
            _leftWall.transform.transform.position = new Vector3(-1, 0, x);

            _rightWall.transform.localScale = new Vector3(1, 10, _currentMapData.Height);
            _rightWall.transform.transform.position = new Vector3(_currentMapData.Width, 0, x);

            _upperWall.transform.localScale = new Vector3(1, 10, _currentMapData.Width);
            _upperWall.transform.transform.position = new Vector3(x, 0, _currentMapData.Height);

            cameraController.SetCamera(_currentMapData.Width, _currentMapData.Height);
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

            destination.X += _dir[(int)moveContext.Direction, 0];
            destination.Z += _dir[(int)moveContext.Direction, 1];

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
            if (data.X < 0 || data.X >= _currentMapData.Width || data.Z < 0 || data.Z >= _currentMapData.Height)
            {
                return false;
            }
            else
                return true;
        }

        //interface
        public void RegisterTileBoard(Tile point, bool isWalkable)
        {
            _tileMapData[GetTileIndex(point.X, point.Z)] = isWalkable;
        }
        private int GetTileIndex(int x, int z) => _currentMapData.Width * z + x;
        private int GetTileIndex(Tile point) => _currentMapData.Width * point.Z + point.X;
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

        public bool IsRouteTile(Tile point)
        {
            if (!IsInArea(point)) return false;
            if (_tileMapData[GetTileIndex(point)] == false) return false;
            if (_obstacleTiles.Contains(point)) return false;
            if (_enemyTiles.Contains(point)) return false;

            return true;
        }
    }

}
