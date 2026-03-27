using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;
using JW.Utility;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stage
{
    public interface IStageService
    {
        event Action OnClearAllFloor;
        event Action OnClearFloor;
        void StartStage();
    }

    public class StageController : MonoBehaviour, IStageService
    {
        [SerializeField] private StageViewer _viewer;

        private MapBundle _currentMapBundle;
        private ShuffleBag<MapData> _actMapBag;

        private int _currentFloor;
        private int _currentAct;

        private int _currentActProgress = 1;
        private int _requireProgress;

        public event Action OnClearAllFloor;
        public event Action OnClearFloor;
        public event Action OnFinishSetStage;
        public int Act => _currentAct + 1;
        public int Floor => _currentFloor + 1;

        IMapService _mapService;
        IFieldObstacleService _obstacleService;
        ITileObject _player;
        IEnemySpawnService _enemySpawnService;

        // 3 floor에 보스

        // 0 1 2 -> index 2에서 보스

        // 3에서 change, reset


        public void Init(IMapService mapService, IFieldObstacleService obstacleService, ITileObject player, IEnemySpawnService enemySpawnService)
        {
            _mapService = mapService;
            _obstacleService = obstacleService;
            _player = player;
            _enemySpawnService = enemySpawnService;

            _currentMapBundle = GameManager.Resource.MapBundle;

            _viewer.Init(_currentMapBundle.TotalFloorCount(), _currentMapBundle.GetBossStages());

            UpdateMapData();
        }
        public void StartStage()
        {
            ClearField();

            UpdateFloorAndAct();

            Debug.Log(_actMapBag);
            MapData map = _actMapBag.GetItem();
            var effectTileData = map.effectTileDatas;

            Debug.Log(map);
            CreatureTemplete templete = GetTemplete(map);

            _mapService.SetMap(map.MapTiles, map.Height, map.Width, map.effectTileDatas);

            _enemySpawnService.ReceiveNomalEnemySpawnList(templete.NomalEnemyPos, Act);

            if(IsBossFloor())
                _enemySpawnService.ReceiveBossEnemySpawnList(templete.BossEnemyPos, Act);

            _player.SetPosition(templete.PlayerPos);

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

            _viewer.UpdateFloor(_currentFloor-1);
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
            Debug.Log($"{_currentActProgress} : {_requireProgress}");
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
