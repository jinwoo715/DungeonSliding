using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;

namespace JW.DungeonSliding.UI
{
    public class HitDamageViewer : MonoBehaviour, IHitDamageUIService
    {
        [SerializeField] private HitDamageItem _hitDamageItem;

        ObjectPool<HitDamageItem> objectPool;

        public void Init()
        {
            objectPool = new ObjectPool<HitDamageItem>(_hitDamageItem, 5, this.transform);
        }

        public void ShowDamage(Vector3 showPosition, int damage)
        {
            var obj = objectPool.GetObject();

            obj.transform.position = Camera.main.WorldToScreenPoint(showPosition);

            obj.Init(damage, 0.5f);
        }
    }
}
