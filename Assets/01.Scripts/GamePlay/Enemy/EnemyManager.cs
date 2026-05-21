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
        int ActiveEnemyCount { get; }
        event Action<Tile, Enemy> OnSpawnEnemy;
        event Action<Tile, Enemy> OnEnemyDeath;
        void ReceiveNomalEnemySpawnList(List<Tile> spawnPositions, int act);
        void ReceiveBossEnemySpawnList(List<Tile> spawnPositions, int act);
    }

    public class EnemyManager : MonoBehaviour, ICombatProvider, IEnemySpawnService
    {
        [SerializeField] private Enemy _enemyPrefabList;

        private Stack<Enemy> _enemyPoolByUID = new Stack<Enemy>();
        
        private Dictionary<Tile, ICombatant> _activeEnemyByTile = new Dictionary<Tile, ICombatant>();

        private List<EnemyData> _nomalEnemyDatas = new();
        private List<EnemyData> _bossEnemyDatas = new();

        private IEnemyAbilityCreater _enemyAbilityCreater;

        public int ActiveEnemyCount => _activeEnemyByTile.Count;

        public event Action<Tile, Enemy> OnSpawnEnemy;
        public event Action<Tile, Enemy> OnEnemyDeath;

        public void Init(IEnemyAbilityCreater enemyAbilityCreater)
        {
            _enemyAbilityCreater = enemyAbilityCreater;

            LoadData();
        }
        public void LoadData()
        {
            _nomalEnemyDatas = GameManager.Data.EnemyData;
            _bossEnemyDatas = GameManager.Data.EnemyBossData;
        }

        private Enemy GetEnemy()
        {
            Enemy enemy = null;

            if (_enemyPoolByUID.Count > 0)
                enemy = _enemyPoolByUID.Pop();
            else
                enemy = InstantiateEnemy();

            enemy.gameObject.SetActive(true);
            enemy.OnEnemyReturnEvent += DeathEnemy;

            return enemy;
        }
        private Enemy InstantiateEnemy()
        {
            Enemy enemy = Instantiate(_enemyPrefabList, this.transform);
            enemy.Initialize(ECreatureType.Enemy);

            return enemy;
        }
        private void DeathEnemy(Enemy enemy)
        {
            ReturnEnemy(enemy);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnEnemyDeath);

            Tile tile = enemy.TileObject.TilePosition;

            _activeEnemyByTile.Remove(tile);
            OnEnemyDeath?.Invoke(tile, enemy);

            if (_activeEnemyByTile.Count == 0)
            {
                Action clearEvent = () => GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnClearStage);
                GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnTurnEnd, clearEvent);
            }
        }
        public void ReturnEnemy(Enemy enemy)
        {
            enemy.OnEnemyReturnEvent -= DeathEnemy;
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
        public void ReceiveNomalEnemySpawnList(List<Tile> spawnPositions, int act)
        {
            int maxRandomEnemyIndex = Mathf.Min(act, _nomalEnemyDatas.Count);

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                int ranNum = UnityEngine.Random.Range(0, maxRandomEnemyIndex);

                Tile tile = spawnPositions[i];

                EnemyData data = _nomalEnemyDatas[ranNum];

                SpawnEnemy(data, act, tile);
            }
        }
        public void ReceiveBossEnemySpawnList(List<Tile> spawnPositions, int act)
        {
            Debug.Log($"Boss Stage {spawnPositions.Count}");
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                int ranNum = UnityEngine.Random.Range(0, _bossEnemyDatas.Count);

                Tile tile = spawnPositions[i];

                EnemyData data = _bossEnemyDatas[ranNum];
                Debug.Log(data.AbilityList);
                SpawnEnemy(data, act, tile);
            }
        }
        private void SpawnEnemy(EnemyData data, int act, Tile spawnPosition)
        {
            Enemy enemy = GetEnemy();
            InitEnemy(enemy, data, act);
            SetEnemyOnTile(enemy, spawnPosition);
            SetEnemySkill(data, enemy, act);
            OnSpawnEnemy?.Invoke(spawnPosition, enemy);
        }
        private void InitEnemy(Enemy enemy, EnemyData data, int act)
        {
            enemy.SetData(data, act);
        }
        private void SetEnemyOnTile(Enemy enemy, Tile tile)
        {
            enemy.TileObject.SetPosition(tile);
            _activeEnemyByTile.Add(tile, enemy);
        }
        private void SetEnemySkill(EnemyData data, Enemy enemy, int act)
        {
            var datas = GameManager.Data.EnemyAbilities(data.AbilityList);

            if (datas != null)
            {
                var abilities = _enemyAbilityCreater.CreateAbility(datas, enemy, act);
                enemy.AbilityRegister.RegisterAutoAllAbility(abilities);
            }
        }
    }
}
