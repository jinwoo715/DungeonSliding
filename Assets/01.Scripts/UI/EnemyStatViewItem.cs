using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;
using JW.Utility;
using JW.DungeonSliding.GamePlay.Combat;
using TMPro;

namespace JW.DungeonSliding 
{
    public class EnemyStatViewItem : PoolObject
    {
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private CretureStat_UI _hpUI;
        [SerializeField] private CretureStat_UI _damageUI;

        private Transform _transform;

        private IStatModifier _statModifier;
        private IStatReadOnly _statReadOnly;
        
        public void Init(Transform target, ICombatant combatant)
        {
            _transform = target.transform;

            _statModifier = combatant.StatModifier;
            _statReadOnly = combatant.StatReadOnly;

            _statModifier.OnStatChanged += UpdateStat;

            UpdateStat(ECreatureStatType.CurrentHP);
            UpdateStat(ECreatureStatType.Damage);

            if (combatant is Enemy enemy)
            {
                _nameText.text = enemy.Name;

                _nameText.color = enemy.IsBoss == true ? Color.red : Color.white;
            }
        }

        private void LateUpdate()
        {
            if (_transform == null) return;

            Vector3 position = Camera.main.WorldToScreenPoint(_transform.position);
            position.z = 0;
            position.y += 50;

            this.transform.position = position;
        }

        public void UpdateStat(ECreatureStatType statType)
        {
            if(statType == ECreatureStatType.CurrentHP)
            {
                _hpUI.UpdateValue(_statReadOnly.Get(ECreatureStatType.CurrentHP));
            }
            else
            {
                _damageUI.UpdateValue(_statReadOnly.Get(ECreatureStatType.Damage));
            }
        }

        public override void OnDespawn()
        {
            _statModifier.OnStatChanged -= UpdateStat;
            _statModifier = null;
            _statReadOnly = null;
        }

        public override void OnSpawn()
        {
            
        }
    }
}
