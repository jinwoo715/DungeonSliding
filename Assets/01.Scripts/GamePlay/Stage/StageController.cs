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

        private MapBundle _mapDataBundle;

        private ShuffleBag<MapData> _actMapBag;

        private int _currentFloor = 0;
        private int _act = 0;
        private int _actOffset = 0;

        public event Action OnClearAllFloor;
        public event Action OnClearFloor;
        public event Action OnFinishSetStage;

        IMapService _mapService;
        IFieldObstacleService _obstacleService;
        ITileObject _player;
        IEnemySpawnService _enemySpawnService;

        public void Init(IMapService mapService, IFieldObstacleService obstacleService, ITileObject player, IEnemySpawnService enemySpawnService)
        {
            _mapService = mapService;
            _obstacleService = obstacleService;
            _player = player;
            _enemySpawnService = enemySpawnService;

            _mapDataBundle = GameManager.Resource.MapBundle;
            _actOffset = GameManager.Config.Act.ActPerFloor;

            UpdateMapData();
        }

        //stage start -> update floor
        
        //IMapService에 넘겨줘야 할 데이터 : 타일 정보, 효과타일 정보

        public void StartStage()
        {
            UpdateFloorAndAct();

            _mapService.ClearMap();
            _obstacleService.ClearObstacles();

            MapData map = _actMapBag.GetItem();
            var effectTileData = map.effectTileDatas;

            _mapService.SetMap(map.MapTiles, map.Height, map.Width, map.effectTileDatas);

            CreatureTemplete templete = GetTemplete(map);

            _enemySpawnService.SpawnNomalEnemies(templete.NomalEnemyPos, _act);

            _player.SetPosition(templete.PlayerPos);

            OnFinishSetStage?.Invoke();
        }

        private void UpdateFloorAndAct()
        {
            _viewer.UpdateFloor(_currentFloor);

            _currentFloor++;

            bool boss = _currentFloor % _actOffset == 0;
            int act = _currentFloor / _actOffset;
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
            var data = _mapDataBundle.GetActMapBundle(_act);
            _actMapBag = new ShuffleBag<MapData>(data.MapDatas);
        }

        private bool IsActChange()
        {
            return false;
        }

        private bool IsBossFloor()
        {
            return false;
        }
    }
}
