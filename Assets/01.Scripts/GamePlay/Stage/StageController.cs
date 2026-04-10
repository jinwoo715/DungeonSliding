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
        public int TotalFloor { get; }
        public List<int> BossFloors { get; }
    }

    public class StageController : MonoBehaviour, IStageService, IStageViewer
    {
        private MapBundle _currentMapBundle;
        private ShuffleBag<MapData> _actMapBag;

        private int _currentFloor;
        private int _currentAct;

        private int _currentActProgress = 1;
        private int _requireProgress;

        public event Action OnClearAllFloor;
        public event Action OnClearFloor;
        public event Action OnFinishSetStage;
        public event Action<int> OnChangeActEvent;
        public event Action<int> OnChangeFloorEvent;

        public int Act => _currentAct + 1;
        public int CurrentFloor => _currentFloor;
        public List<int> BossFloors => _currentMapBundle.GetBossStages();
        public int TotalFloor => _currentMapBundle.TotalFloorCount();

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
            ClearField();

            UpdateFloorAndAct();

            MapData map = _actMapBag.GetItem();
            var effectTileData = map.effectTileDatas;

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
            _currentActProgress++;
            _currentFloor++;

            if (TryUpdateAct())
            {
                UpdateMapData();
            }
            OnChangeFloorEvent?.Invoke(_currentAct - 1);
        }
        private bool TryUpdateAct()
        {
            if(_currentActProgress > _requireProgress)
            {
                _currentAct++;
                return true;
            }

            return false;
        }
        private bool IsBossFloor()
        {
            return _currentActProgress == _requireProgress;
        }
        private CreatureTemplete GetTemplete(MapData map)
        {
            var enemyTemplete = map.CretureTempletes;
            int ranNum = Chance.GetRandomNum(enemyTemplete.Count);
            CreatureTemplete templete = enemyTemplete[ranNum];
            return templete;
        }
        private void UpdateMapData()
        {
            var data = _currentMapBundle.GetActMapBundle(_currentAct);
            _actMapBag = new ShuffleBag<MapData>(data.MapDatas);

            _currentActProgress--;
            _requireProgress = data.ActFloorCount;
        }
    }
}
