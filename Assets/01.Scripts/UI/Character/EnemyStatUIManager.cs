using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections.Generic;

namespace JW.DungeonSliding
{
    public class EnemyStatUIManager : MonoBehaviour, IEnemyStatUIService
    {
        [SerializeField] private EnemyStatViewItem _enemyStatViewItem;
        
        private ObjectPool<EnemyStatViewItem> _statViewItemPool;
        private Dictionary<IEnemyStatReadOnly, EnemyStatViewItem> _activeEnemyStatItemByEnemy = new();
        public void Init()
        {
            _statViewItemPool = new ObjectPool<EnemyStatViewItem>(_enemyStatViewItem, 3, this.transform);
        }

        public void ReleaseStatViewItem(EnemyStatViewItem item)
        {
            item.Release();
        }

        public void Attach(Transform transform, IEnemyStatReadOnly enemyStatReadOnly)
        {
            EnemyStatViewItem item = _statViewItemPool.GetObject();
            item.Init(transform, enemyStatReadOnly);
            _activeEnemyStatItemByEnemy.Add(enemyStatReadOnly, item);
        }

        public void Detach(IEnemyStatReadOnly enemyStatReadOnly)
        {
            if(_activeEnemyStatItemByEnemy.TryGetValue(enemyStatReadOnly, out var value))
            {
                _activeEnemyStatItemByEnemy[enemyStatReadOnly].Release();
                _activeEnemyStatItemByEnemy.Remove(enemyStatReadOnly);
            }
        }
    }
}
