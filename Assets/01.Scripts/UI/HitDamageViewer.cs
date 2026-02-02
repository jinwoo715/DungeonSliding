using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.UI
{
    public class HitDamageViewer : MonoBehaviour
    {
        [SerializeField] private HitDamageItem _hitDamageItem;

        ObjectPool<HitDamageItem> objectPool;

        public void Init(ICombatEventPresenter combatEventPresenter)
        {
            objectPool = new ObjectPool<HitDamageItem>(_hitDamageItem, 5, this.transform);

            combatEventPresenter.DamageEvent += ShowDamage;
        }

        public void ShowDamage(DamageEvent damageEvent)
        {
            var obj = objectPool.GetObject();

            Vector3 spawnPosition = damageEvent.Target.TilePosition.GetPosition;
            spawnPosition += Vector3.up;

            obj.transform.position = Camera.main.WorldToScreenPoint(spawnPosition);

            obj.Init(damageEvent.Damage, 0.5f);
        }
    }
}
