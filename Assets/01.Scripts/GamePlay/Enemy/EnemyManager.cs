using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.GamePlay.Context;
using System;
using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Stats;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class EnemyManager : MonoBehaviour, ICombatProvider
    {
        [SerializeField] private CretureStatController _cretureStatController;
        [SerializeField] private List<Enemy> _enemyPrefabList;

        private Dictionary<int, Stack<Enemy>> _enemyPoolByUID = new Dictionary<int, Stack<Enemy>>();
        
        private Dictionary<Tile, ICombatant> _activeEnemyByTile = new Dictionary<Tile, ICombatant>();

        private Dictionary<int, EnemyData> _enemyDataByUID = new Dictionary<int, EnemyData>();

        public event Action<RewardData> OnEnemyRewardEvent;

        private IBoard _board;
        private IObstacleRequest _obstacleRequest;
        private IEnemyStatUIService _enemyStatUIService;
        private ICombatEventListener _combatEventListener;
        public void WireInterfaces(IBoard board, IObstacleRequest obstacleRequest, IEnemyStatUIService enemyStatUIService,
            ICombatEventListener combatEventListener)
        {
            _board = board;
            _obstacleRequest = obstacleRequest;
            _enemyStatUIService = enemyStatUIService;
            _combatEventListener = combatEventListener;
        }
        public void LoadData()
        {
            string enemyJsonData = GameManager.Instance.Resource.GetTextData("EnemyData");
            var enemyDatas = JsonConvert.DeserializeObject<List<EnemyData>>(enemyJsonData);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                _enemyDataByUID[i] = enemyDatas[i];
            }
        }

        public void SetEnemy(EnemyTemplete[] enemyTempletes, int floor)
        {
            int templeteNum = UnityEngine.Random.Range(0, enemyTempletes.Length);
            EnemyTemplete templete = enemyTempletes[templeteNum];

            for (int i = 0; i < templete.EnemyData.Count; i++)
            {
                EnemySettingData data = templete.EnemyData[i];
                Enemy enemy = GetEnemy(data.EnemyUID);
                enemy.SetData(_enemyDataByUID[data.EnemyUID], floor);
                
                enemy.SetPosition(data.Point);

                _activeEnemyByTile.Add(data.Point, enemy);
                _board.RegisterEnemyTile(data.Point);
            }
        }
        private Enemy GetEnemy(int enemyUID)
        {
            Enemy enemy = null;

            if(_enemyPoolByUID.TryGetValue(enemyUID, out Stack<Enemy> pool))
            {
                if (pool.Count > 0)
                    enemy = pool.Pop();
                else
                    enemy = SpawnEnemy(enemyUID);
            }
            else
            {
                _enemyPoolByUID[enemyUID] = new Stack<Enemy>();
                enemy = SpawnEnemy(enemyUID);
            }

            enemy.gameObject.SetActive(true);
            _enemyStatUIService.Attach(enemy.StatUITransform, enemy);

            return enemy;
        }
        private Enemy SpawnEnemy(int enemyUID)
        {
            Enemy enemy = Instantiate(_enemyPrefabList[enemyUID], this.transform);
            enemy.OnDeathEvent += OnEnemyDeath;
            enemy.Init(_combatEventListener, ECretureType.Enemy);

            return enemy;
        }
        public void ReturnEnemy(Enemy enemy)
        {
            enemy.gameObject.SetActive(false);
            _enemyPoolByUID[enemy.EnemyUID].Push(enemy);
        }
        private void OnEnemyDeath(Enemy enemy)
        {
            ReturnEnemy(enemy);

            _activeEnemyByTile.Remove(enemy.TilePosition);
            _obstacleRequest.SpawnObstacle(enemy.TilePosition, EObstacleObjectType.Rubble);
            _enemyStatUIService.Detach(enemy);
            _board.UnRegisterEnemyTile(enemy.TilePosition);

            if (_activeEnemyByTile.Count == 0)
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnClearStage);
            }
        }

        //Interface
        public bool TryGetCombatant(Tile tilePoint, out ICombatant combatant)
        {
            if(_activeEnemyByTile.TryGetValue(tilePoint, out ICombatant value))
            {
                combatant = value;
                return true;
            }
            else
            {
                combatant = value;
                return false;
            }
        }
        public List<ICombatant> GetAllActiveCombatant()
        {
            List<ICombatant> activeList = new List<ICombatant>();
            foreach (var enemy in _activeEnemyByTile)
            {
                activeList.Add(enemy.Value);
            }
            return activeList;
        }
    }
}
