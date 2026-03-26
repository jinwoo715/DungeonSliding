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
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.GamePlay.Entities
{

    public interface IEnemySpawnService
    {
        void SpawnNomalEnemies(List<Tile> spawnPositions, int act);
        void SpawnBossEnemies(List<Tile> spawnPositions, int act);
    }

    public class EnemyManager : MonoBehaviour, ICombatProvider, IEnemySpawnService
    {
        [SerializeField] private Enemy _enemyPrefabList;

        private Stack<Enemy> _enemyPoolByUID = new Stack<Enemy>();
        
        private Dictionary<Tile, ICombatant> _activeEnemyByTile = new Dictionary<Tile, ICombatant>();

        private Dictionary<string, EnemyData> _enemyDataByUID = new();
        private Dictionary<string, EnemyData> _enemyBossDataByUID = new();

        private List<EnemyData> _nomalEnemyDatas = new();
        private List<EnemyData> _bossEnemyDatas = new();

        private IBoard _board;
        private IFieldObstacleService _obstacleRequest;
        private IEnemyStatUIService _enemyStatUIService;
        private ICombatEventListener _combatEventListener;
        private IRequesterRegistry _requesterRegistry;
        private IEnemyAbilityCreater _enemyAbilityCreater;
        public void WireInterfaces(IBoard board, IFieldObstacleService obstacleRequest, IEnemyStatUIService enemyStatUIService,
            ICombatEventListener combatEventListener, IRequesterRegistry requesterRegistry, IEnemyAbilityCreater enemyAbilityCreater)
        {
            _board = board;
            _obstacleRequest = obstacleRequest;
            _enemyStatUIService = enemyStatUIService;
            _combatEventListener = combatEventListener;
            _requesterRegistry = requesterRegistry;
            _enemyAbilityCreater = enemyAbilityCreater;
        }
        public void LoadData()
        {
            _nomalEnemyDatas = GameManager.Data.EnemyData;
            _bossEnemyDatas = GameManager.Data.EnemyBossData;

            for (int i = 0; i < _nomalEnemyDatas.Count; i++)
            {
                _enemyDataByUID[_nomalEnemyDatas[i].UID] = _nomalEnemyDatas[i];
            }

            for (int i = 0; i < _bossEnemyDatas.Count; i++)
            {
                _enemyBossDataByUID[_bossEnemyDatas[i].UID] = _bossEnemyDatas[i];
            }
        }

        public int spawnNum;
        internal void SpawnEnemy(List<Tile> NomalEnemyPos, List<Tile> BossEnemyPos, int act, int floor) 
        { 
            if (floor+1 % 3 != 0) return;

            for (int i = 0; i < BossEnemyPos.Count; i++)
            {
                Tile tile = BossEnemyPos[i];

                Enemy boss = GetEnemy();
                var data = _enemyBossDataByUID["ENEMY_BOSS_EREBOS"];
                boss.SetData(data, 1);

                CreatureBaseStat baseStat = new CreatureBaseStat(data.BaseHP, data.BaseDamage, 100);
                boss.InitData(baseStat);
                boss.RegisterRequester(_requesterRegistry);
                boss.Tile.SetPosition(tile);

                _activeEnemyByTile.Add(tile, boss);
                _board.RegisterEnemyTile(tile);
            }
        }

        private Enemy GetEnemy()
        {
            Enemy enemy = null;

            if (_enemyPoolByUID.Count > 0)
                enemy = _enemyPoolByUID.Pop();
            else
                enemy = InstantiateEnemy();

            enemy.gameObject.SetActive(true);
            _enemyStatUIService.Attach(enemy.StatUITransform, enemy);

            enemy.OnEnemyReturnEvent += OnEnemyDeath;

            return enemy;
        }
        private Enemy InstantiateEnemy()
        {
            Enemy enemy = Instantiate(_enemyPrefabList, this.transform);
            enemy.Initialize(ECreatureType.Enemy);

            return enemy;
        }
        private void OnEnemyDeath(Enemy enemy)
        {
            ReturnEnemy(enemy);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnEnemyDeath);

            Tile tile = enemy.Tile.TilePosition;

            _activeEnemyByTile.Remove(tile);
            _obstacleRequest.SpawnObstacle(tile, EObstacleObjectType.Rubble);
            _enemyStatUIService.Detach(enemy);
            _board.UnRegisterEnemyTile(tile);

            if (_activeEnemyByTile.Count == 0)
            {
                Action clearEvent = () => GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnClearStage);
                GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnTurnEnd, clearEvent);
            }
        }
        public void ReturnEnemy(Enemy enemy)
        {
            enemy.OnEnemyReturnEvent -= OnEnemyDeath;
            enemy.gameObject.SetActive(false);
            _enemyPoolByUID.Push(enemy);
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
        public void SpawnNomalEnemies(List<Tile> spawnPositions, int act)
        {
            int maxNum = Mathf.Min(act, _nomalEnemyDatas.Count);

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                int ranNum = UnityEngine.Random.Range(0, maxNum);

                Tile tile = spawnPositions[i];

                EnemyData data = _nomalEnemyDatas[spawnNum];

                Enemy enemy = GetEnemy();
                enemy.SetData(data, act);
                enemy.RegisterRequester(_requesterRegistry);
                enemy.Tile.SetPosition(tile);

                var datas = GameManager.Data.EnemyAbilities(data.AbilityList);

                if (datas != null)
                {
                    var abilities = _enemyAbilityCreater.CreateAbility(datas, enemy, act);
                    enemy.AbilityRegister.RegisterAutoAllAbility(abilities);
                }

                _activeEnemyByTile.Add(tile, enemy);
                _board.RegisterEnemyTile(tile);
            }
        }
        public void SpawnBossEnemies(List<Tile> spawnPositions, int act)
        {
            
        }
    }
}
