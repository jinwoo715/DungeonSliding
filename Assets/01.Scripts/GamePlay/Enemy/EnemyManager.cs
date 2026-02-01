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

        private IAttackRequestListener _attackRequestListener;
        private ICombatantSensor _combatantSensor;
        private IBoard _board;
        private IObstacleRequest _obstacleRequest;
        private IEnemyStatUIService _enemyStatUIService;
        private IHitDamageUIService _hitDamageUIService;
        public void WireInterfaces(IBoard board, IAttackRequestListener attackRequestListener,
            ICombatantSensor sensor, IObstacleRequest obstacleRequest, IEnemyStatUIService enemyStatUIService,
            IHitDamageUIService hitDamageUIService)
        {
            _board = board;
            _attackRequestListener = attackRequestListener;
            _combatantSensor = sensor;
            _obstacleRequest = obstacleRequest;
            _enemyStatUIService = enemyStatUIService;
            _hitDamageUIService = hitDamageUIService;
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
                enemy.Init();
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
            _enemyStatUIService.Attach(enemy.UITransform, enemy);

            return enemy;
        }
        private Enemy SpawnEnemy(int enemyUID)
        {
            Enemy enemy = Instantiate(_enemyPrefabList[enemyUID], this.transform);
            enemy.ReturnEvent = ReturnEnemy;
            enemy.OnDeathEvent = OnEnemyDeath;
            enemy.SetShowHitDamageUIEvent(_hitDamageUIService.ShowDamage);
            enemy.SetAttackRequestListener(_attackRequestListener);
            enemy.SetCombatSensor(_combatantSensor);
            return enemy;
        }
        public void ReturnEnemy(Enemy enemy)
        {
            enemy.gameObject.SetActive(false);
            _enemyPoolByUID[enemy.EnemyUID].Push(enemy);
        }
        private void OnEnemyDeath(Enemy enemy)
        {
            OnEnemyRewardEvent?.Invoke(new RewardData(enemy.Xp));
            _activeEnemyByTile.Remove(enemy.TilePosition);

            _obstacleRequest.SpawnObstacle(enemy.TilePosition, EObstacleObjectType.Rubble);

            _enemyStatUIService.Detach(enemy);

            if (_activeEnemyByTile.Count == 0)
            {
                Debug.Log("All Clear!");
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.ClearStage);
            }

            _board.UnRegisterEnemyTile(enemy.TilePosition);
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
