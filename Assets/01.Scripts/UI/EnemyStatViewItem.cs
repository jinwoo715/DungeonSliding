using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding 
{
    public class EnemyStatViewItem : PoolObject
    {
        private Transform _targetTransform;
        [SerializeField] private Vector3 _offset;

        [SerializeField] private CretureStat_UI _hpUI;
        [SerializeField] private CretureStat_UI _damageUI;

        private IEnemyStatReadOnly _statReadOnly;
        
        public void Init(Transform target, IEnemyStatReadOnly statReadOnly)
        {
            _targetTransform = target.transform;
            _statReadOnly = statReadOnly;

            _statReadOnly.OnStatChanged += UpdateStat;
        }

        private void LateUpdate()
        {
            //if (_targetTransform == null)
            //{
            //    Release();
            //}
            //else
            {
                transform.forward = Camera.main.transform.forward;

                Vector3 position = Camera.main.WorldToScreenPoint(_targetTransform.position);
                position.z = 0;

                this.transform.position = position;
            }
        }

        public void UpdateStat(EEnemyStatType statType)
        {
            if(statType == EEnemyStatType.HP)
            {
                _hpUI.UpdateValue(_statReadOnly.Get(EEnemyStatType.HP));
            }
            else
            {
                _damageUI.UpdateValue(_statReadOnly.Get(EEnemyStatType.Damage));
            }
        }

        public override void OnDespawn()
        {
            _statReadOnly.OnStatChanged -= UpdateStat;
            _statReadOnly = null;
        }

        public override void OnSpawn()
        {

        }
    }
}
