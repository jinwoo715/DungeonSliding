using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding 
{
    public class EnemyStatViewItem : PoolObject
    {
        [SerializeField] private CretureStat_UI _hpUI;
        [SerializeField] private CretureStat_UI _damageUI;

        private Transform _transform;

        private IEnemyStatModifier _statModifier;
        
        public void Init(Transform target, IEnemyStatModifier statReadOnly)
        {
            _transform = target.transform;
            _statModifier = statReadOnly;
        }

        private void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;

            Vector3 position = Camera.main.WorldToScreenPoint(_transform.position);
            position.z = 0;

            this.transform.position = position;
        }

        public void UpdateStat(EEnemyStatType statType)
        {
            if(statType == EEnemyStatType.HP)
            {
                _hpUI.UpdateValue(_statModifier.Get(EEnemyStatType.HP));
            }
            else
            {
                _damageUI.UpdateValue(_statModifier.Get(EEnemyStatType.Damage));
            }
        }

        public override void OnDespawn()
        {
            _statModifier.OnStatChangedEvent -= UpdateStat;
            _statModifier = null;
        }

        public override void OnSpawn()
        {
            _statModifier.OnStatChangedEvent += UpdateStat;
        }
    }
}
