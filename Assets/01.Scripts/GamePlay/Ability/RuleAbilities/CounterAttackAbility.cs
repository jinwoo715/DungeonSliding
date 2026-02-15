using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class CounterAttackAbility : AbilityBase
    {
        IDamageable _damageable;
        ICounterAttackable _counterAttackable;
        public CounterAttackAbility(RuleAbilityData data, AbilityHost host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            _counterAttackable.RequestCounterAttack(_damageable.LastAttacker);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnDamaged)
            {
                int chanceValue = Random.Range(0, 101);

                Debug.Log(chanceValue);

                if (chanceValue <= _data.P1)
                {
                    ExcuteAbility();
                }
            }
        }

        protected override void BindService()
        {
            BindService<ICounterAttackable>(ref _counterAttackable);
            BindService<IDamageable>(ref _damageable);

            Debug.Log(_damageable);
            Debug.Log(_counterAttackable);
        }
    }
}