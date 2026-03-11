using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections.Generic;
using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding
{
    public class EnemyStatPresenter : MonoBehaviour, IEnemyStatUIService
    {
        [SerializeField] private EnemyStatViewItem _enemyStatViewItem;
        
        private ObjectPool<EnemyStatViewItem> _statViewItemPool;
        private Dictionary<ICombatant, EnemyStatViewItem> _activeEnemyStatItemByEnemy = new();
        public void Init()
        {
            _statViewItemPool = new ObjectPool<EnemyStatViewItem>(_enemyStatViewItem, 3, this.transform);
        }

        public void ReleaseStatViewItem(EnemyStatViewItem item)
        {
            item.Release();
        }

        public void Attach(Transform transform, ICombatant combatant)
        {
            EnemyStatViewItem item = _statViewItemPool.GetObject();
            item.Init(transform, combatant);

            _activeEnemyStatItemByEnemy.Add(combatant, item);
        }

        public void Detach(ICombatant enemyStatReadOnly)
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
