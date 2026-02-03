using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using System.Collections.Generic;

namespace JW.DungeonSliding.UI
{
    public class HitDamageViewer : MonoBehaviour
    {
        [SerializeField] private HitDamageItem _hitDamageItem;

        private ObjectPool<HitDamageItem> objectPool;
        private Queue<DamageEvent> _showDamageUIStack = new Queue<DamageEvent>();
        private Coroutine _showUICoroutine;

        public void Init(ICombatEventPresenter combatEventPresenter)
        {
            objectPool = new ObjectPool<HitDamageItem>(_hitDamageItem, 5, this.transform);

            combatEventPresenter.DamageEvent += ShowDamage;
        }

        public void ShowDamage(DamageEvent damageEvent)
        {
            _showDamageUIStack.Enqueue(damageEvent);

            if(_showUICoroutine == null)
                _showUICoroutine = StartCoroutine(CoSpawnDamageUI());
        }

        private IEnumerator CoSpawnDamageUI()
        {
            while (_showDamageUIStack.Count > 0)
            {
                DamageEvent damageEvent = _showDamageUIStack.Dequeue();

                var obj = objectPool.GetObject();

                Vector3 spawnPosition = damageEvent.Target.TilePosition.GetPosition;
                spawnPosition += Vector3.up;

                obj.transform.position = Camera.main.WorldToScreenPoint(spawnPosition);

                obj.Init(damageEvent.Damage, 0.5f);

                yield return new WaitForSeconds(0.2f);
            }

            _showUICoroutine = null;
        }
    }
}
