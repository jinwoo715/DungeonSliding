using JW.DungeonSliding.GamePlay.Stats;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class AttackEnhancer : INextAttackEnhancer
    {
        public event Action<int> OnChangedNextAttackDamage;
        public event Action<int> OnChangedNextAttackCount;

        private int _extraFixedAddDamage = 0;
        private float _extraMultiplierDamageRatio = 0;
        private int _extraAttackCount = 0;
        private bool _guaranteedCritical = false;

        private IStatReadOnly _statReadOnly;

        public void Init(IStatReadOnly statReadOnly)
        {
            _statReadOnly = statReadOnly;
        }
        public int FixedAddDamage => _extraFixedAddDamage;
        public float MultipleAddDamage => _extraMultiplierDamageRatio;
        public int FinalExtraAttackCount => _extraAttackCount;
        public int FinalEnhanceDamage => CalculateFinalExtraDamage();

        public bool IsGuaranteedCritical => throw new NotImplementedException();

        public void AddNextAttackCount(int count)
        {
            _extraAttackCount += count;
            OnChangedNextAttackCount?.Invoke(_extraAttackCount);
        }
        public void AddNextAttackDamage(int damage)
        {
            Debug.Log($"Enhancer Value {damage} ");
            _extraFixedAddDamage += damage;
            CalculateFinalExtraDamage();
        }
        public void AddNextAttackDamageMulti(float multi)
        {
            _extraMultiplierDamageRatio += multi;
            CalculateFinalExtraDamage();
        }
        public void GuaranteedCritical()
        {
            _guaranteedCritical = true;
        }
        public int CalculateFinalExtraDamage()
        {
            int finalDamage = 0;
            finalDamage += _extraFixedAddDamage;

            int baseDamage = _statReadOnly.Get(ECreatureStatType.Damage);

            finalDamage += Mathf.RoundToInt(baseDamage * _extraMultiplierDamageRatio);

            OnChangedNextAttackDamage?.Invoke(finalDamage);

            return finalDamage;
        }
        public void Clear()
        {
            _extraFixedAddDamage = 0;
            _extraAttackCount = 0;
            _extraMultiplierDamageRatio = 0;
            _guaranteedCritical = false;

            OnChangedNextAttackDamage?.Invoke(0);
            OnChangedNextAttackCount?.Invoke(0);
        }
    }
}
