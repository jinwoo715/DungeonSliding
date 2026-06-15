using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;
using JW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stage
{
    public interface IStageService
    {
        event Action OnClearAllFloor;
        event Action OnClearFloor;
        event Action<int> OnChangeActEvent;
        event Action<int> OnChangeFloorEvent;
        void StartStage();
    }

    public interface IStageViewer
    {
        public int CurrentFloor { get; }
        public int MaxFloor { get; }
        public int TotalFloor { get; }
        public List<int> BossFloors { get; }
    }

    public class StageController : MonoBehaviour, IStageService, IStageViewer
    {
        private MapBundle _currentMapBundle;
        private ShuffleBag<MapData> _actMapBag;

        private int _currentFloor;
        private int _currentAct;
        private int _nextBossFloor;

        public event Action OnClearAllFloor;
        public event Action OnClearFloor;
        public event Action OnFinishSetStage;
        public event Action<int> OnChangeActEvent;
        public event Action<int> OnChangeFloorEvent;

        public int Act => _currentAct + 1;
        public int CurrentFloor => _currentFloor;
        public List<int> BossFloors => _currentMapBundle.GetBossStages();
        public int TotalFloor => _currentMapBundle.TotalFloorCount();

        public int MaxFloor => TotalFloor;

        IMapService _mapService;
        IFieldObstacleService _obstacleService;
        IEnemySpawnService _enemySpawnService;
        ICombatant _player;

        public void Init(IMapService mapService, IFieldObstacleService obstacleService, ICombatant player, IEnemySpawnService enemySpawnService)
        {
            _mapService = mapService;
            _obstacleService = obstacleService;
            _player = player;
            _enemySpawnService = enemySpawnService;

            _currentMapBundle = GameManager.Resource.MapBundle;

            UpdateMapData();
        }

        public void CheckStageClear()
        {
            if (_enemySpawnService.ActiveEnemyCount == 0)
                OnClearFloor?.Invoke();
        }

        public void StartStage()
        {
            if (_currentFloor >= _currentMapBundle.TotalFloorCount())
            {
                OnClearAllFloor?.Invoke();
                return;
            }

            ClearField();

            UpdateFloorAndAct();

            MapData map = _actMapBag.GetItem();
            CreatureTemplete templete = GetTemplete(map);

            _mapService.SetMap(map.MapTiles, map.Height, map.Width, map.effectTileDatas);

            _enemySpawnService.ReceiveNomalEnemySpawnList(templete.NomalEnemyPos, Act);

            if(IsBossFloor())
                _enemySpawnService.ReceiveBossEnemySpawnList(templete.BossEnemyPos, Act);

            _player.TileObject.SetPosition(templete.PlayerPos);

            OnFinishSetStage?.Invoke();
        }
        private void ClearField()
        {
            _mapService.ClearMap();
            _obstacleService.ClearObstacles();
        }
        private void UpdateFloorAndAct()
        {
            _currentFloor++;

            if (TryUpdateAct())
            {
                UpdateMapData();
            }
            OnChangeFloorEvent?.Invoke(_currentFloor);
        }
        private bool TryUpdateAct()
        {
            if(_currentFloor > _nextBossFloor)
            {
                _currentAct++;
                OnChangeActEvent?.Invoke(Act);
                return true;
            }

            return false;
        }
        private bool IsBossFloor()
        {
            return _currentFloor == _nextBossFloor;
        }
        private CreatureTemplete GetTemplete(MapData map)
        {
            List<CreatureTemplete> validTempletes = map.CretureTempletes.FindAll(
                templete => templete != null);

            if (validTempletes.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Map '{map.name}' does not contain a valid creature templete.");
            }

            int ranNum = Chance.GetRandomNum(validTempletes.Count);
            return validTempletes[ranNum];
        }
        private void UpdateMapData()
        {
            var data = _currentMapBundle.GetActMapBundle(_currentAct);
            if (data == null)
            {
                throw new InvalidOperationException($"Map bundle for Act {Act} does not exist.");
            }

            List<MapData> validMaps = data.MapDatas?.FindAll(
                map => map != null &&
                       map.MapTiles != null &&
                       map.CretureTempletes != null &&
                       map.CretureTempletes.Count > 0);

            if (validMaps == null || validMaps.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Map bundle for Act {Act} does not contain a valid map.");
            }

            int invalidMapCount = data.MapDatas.Count - validMaps.Count;
            if (invalidMapCount > 0)
            {
                Debug.LogWarning(
                    $"Act {Act} MapBundle contains {invalidMapCount} invalid map reference(s). They will be ignored.");
            }

            _actMapBag = new ShuffleBag<MapData>(validMaps);

            _nextBossFloor += data.ActFloorCount;
        }
    }
}
