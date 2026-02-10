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
        [SerializeField] private List<EnemyBossBase> _enemyBossPrefabList;

        private Dictionary<string, Stack<Enemy>> _enemyPoolByUID = new Dictionary<string, Stack<Enemy>>();
        
        private Dictionary<Tile, ICombatant> _activeEnemyByTile = new Dictionary<Tile, ICombatant>();

        private Dictionary<string, EnemyData> _enemyDataByUID = new();
        private Dictionary<string, EnemyBossData> _enemyBossDataByUID = new();

        public event Action<RewardData> OnEnemyRewardEvent;

        private IBoard _board;
        private IObstacleRequest _obstacleRequest;
        private IEnemyStatUIService _enemyStatUIService;
        private ICombatEventListener _combatEventListener;
        private IEnemyAbilityGetter _bossAbilityGetter;
        public void WireInterfaces(IBoard board, IObstacleRequest obstacleRequest, IEnemyStatUIService enemyStatUIService,
            ICombatEventListener combatEventListener, IEnemyAbilityGetter bossAbilityGetter)
        {
            _board = board;
            _obstacleRequest = obstacleRequest;
            _enemyStatUIService = enemyStatUIService;
            _combatEventListener = combatEventListener;
            _bossAbilityGetter = bossAbilityGetter;
        }
        public void LoadData()
        {
            string enemyJsonData = GameManager.Instance.Resource.GetTextData("EnemyData");
            string enemyBossJsonData = GameManager.Instance.Resource.GetTextData("EnemyBossData");

            var enemyDatas = JsonConvert.DeserializeObject<List<EnemyData>>(enemyJsonData);
            //var enemyBossDatas = JsonConvert.DeserializeObject<List<EnemyBossData>>(enemyBossJsonData);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                //_enemyDataByUID[enemyDatas[i].UID] = enemyDatas[i];
            }

            //for (int i = 0; i < enemyBossDatas.Count; i++)
            //{
            //    _enemyBossDataByUID[enemyBossDatas[i].UID] = enemyBossDatas[i];
            //}
        }

        public void SetEnemy(EnemyTemplete[] enemyTempletes, int floor)
        {
            int templeteNum = UnityEngine.Random.Range(0, enemyTempletes.Length);
            EnemyTemplete templete = enemyTempletes[templeteNum];

            for (int i = 0; i < templete.EnemyData.Count; i++)
            {
                EnemySettingData data = templete.EnemyData[i];

                Enemy boss = Instantiate(_enemyPrefabList[0]);
                //boss.SetData(_enemyDataByUID[data.EnemyUID], floor);
                boss.SetData(new EnemyData(), floor);

                boss.SetPosition(data.Point);
                boss.Init(_combatEventListener, ECretureType.Enemy);
                _enemyStatUIService.Attach(boss.StatUITransform, boss);
                IBossAbility bossAbility = boss as IBossAbility;
                if (bossAbility != null)
                {
                    bossAbility.SetAbilityGetter(_bossAbilityGetter);
                }
                _activeEnemyByTile.Add(data.Point, boss);
                _board.RegisterEnemyTile(data.Point);
                //                Enemy enemy = GetEnemy(data.EnemyUID);
                //                enemy.SetData(_enemyDataByUID[data.EnemyUID], floor);
                //                enemy.SetPosition(data.Point);

                //_activeEnemyByTile.Add(data.Point, enemy);
                //_board.RegisterEnemyTile(data.Point);
            }
        }
        private Enemy GetEnemy(string enemyUID)
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
        private Enemy SpawnEnemy(string enemyUID)
        {
            Enemy enemy = Instantiate(_enemyPrefabList[0], this.transform);
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
