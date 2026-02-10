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
        private Dictionary<IEnemyStatModifier, EnemyStatViewItem> _activeEnemyStatItemByEnemy = new();
        public void Init()
        {
            _statViewItemPool = new ObjectPool<EnemyStatViewItem>(_enemyStatViewItem, 3, this.transform);
        }

        public void ReleaseStatViewItem(EnemyStatViewItem item)
        {
            item.Release();
        }

        public void Attach(Transform transform, IEnemyStatModifier enemyStatReadOnly)
        {
            EnemyStatViewItem item = _statViewItemPool.GetObject();
            item.Init(transform, enemyStatReadOnly);
            _activeEnemyStatItemByEnemy.Add(enemyStatReadOnly, item);
        }

        public void Detach(IEnemyStatModifier enemyStatReadOnly)
        {
            if(_activeEnemyStatItemByEnemy.TryGetValue(enemyStatReadOnly, out var value))
            {
                _activeEnemyStatItemByEnemy[enemyStatReadOnly].Release();
                _activeEnemyStatItemByEnemy.Remove(enemyStatReadOnly);
            }
        }

        public void HideAll()
        {
            foreach (var ui in _activeEnemyStatItemByEnemy)
            {
                ui.Value.gameObject.SetActive(false);
            }
        }
        public void ShowAll()
        {
            foreach (var ui in _activeEnemyStatItemByEnemy)
            {
                ui.Value.gameObject.SetActive(true);
            }
        }
    }
}
