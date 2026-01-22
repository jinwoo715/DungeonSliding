using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

namespace JW.SlidingPuzzle {
    public class EnemyManager : MonoBehaviour, ICombatProvider
    {
        [SerializeField] private CretureStatController _cretureStatController;
        [SerializeField] private List<Enemy> _enemyPrefabList;

        private Dictionary<int, Stack<Enemy>> _enemyPoolDic = new Dictionary<int, Stack<Enemy>>();
        private Dictionary<TilePoint, ICombatant> _activeEnemyDic = new Dictionary<TilePoint, ICombatant>();

        private Dictionary<int, EnemyData> _enemyDataDic = new Dictionary<int, EnemyData>();

        private IBoard _board;
        int templeteNum = 0;

        [SerializeField] private TextAsset _enemyData;

        public void SetBoard(IBoard board)
        {
            _board = board;

            var enemyDatas = JsonConvert.DeserializeObject<List<EnemyData>>(_enemyData.text);

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                _enemyDataDic[i] = enemyDatas[i];
            }
        }

        public void SetEnemy(EnemyTemplete[] enemyTempletes)
        {
            templeteNum = Random.Range(0, enemyTempletes.Length);
            EnemyTemplete templete = enemyTempletes[templeteNum];

            int floor = GameSceneManager.Instance.Floor;

            for (int i = 0; i < templete.EnemyData.Count; i++)
            {
                EnemySettingData data = templete.EnemyData[i];
                Enemy enemy = GetEnemy(data.EnemyUID);
                enemy.SetData(_enemyDataDic[data.EnemyUID], floor);
                enemy.Init();
                enemy.SetPosition(data.Point);
                
                _activeEnemyDic.Add(data.Point, enemy);
                _board.RegisterEnemyBoard(data.Point, enemy);
            }
        }
        private Enemy GetEnemy(int enemyUID)
        {
            if(_enemyPoolDic.TryGetValue(enemyUID, out Stack<Enemy> pool))
            {
                if (pool.Count > 0)
                    return pool.Pop();
                else
                    return SpawnEnemy(enemyUID);
            }
            else
            {
                _enemyPoolDic[enemyUID] = new Stack<Enemy>();
                return SpawnEnemy(enemyUID);
            }
        }
        private Enemy SpawnEnemy(int enemyUID)
        {
            Enemy enemy = Instantiate(_enemyPrefabList[enemyUID], this.transform);
            enemy.ReturnEvent = ReturnEnemy;
            return enemy;
        }
        public void ReturnEnemy(Enemy enemy)
        {
            enemy.gameObject.SetActive(false);

            _enemyPoolDic[enemy.EnemyUID].Push(enemy);

            _activeEnemyDic.Remove(enemy.Point);

            _board.UnRegisterEnemyBoard(enemy.Point);
        }

        //Interface
        public bool TryGetCombatant(TilePoint tilePoint, out ICombatant combatant)
        {
            if(_activeEnemyDic.TryGetValue(tilePoint, out ICombatant value))
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
        public List<ICombatant> GetAllCombatant()
        {
            List<ICombatant> activeList = new List<ICombatant>();
            foreach (var enemy in _activeEnemyDic)
            {
                activeList.Add(enemy.Value);
            }
            return activeList;
        }
    }
}
