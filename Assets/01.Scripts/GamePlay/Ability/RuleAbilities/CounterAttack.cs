using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class CounterAttack : RuleAbility
    {
        IDamageable _idamageable;
        ICounterAttackable _counterAttackable;
        public CounterAttack(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            BindService<ICounterAttackable>(ref _counterAttackable);
            BindService<IDamageable>(ref _idamageable);
        }

        public override void ExcuteAbility()
        {
            _counterAttackable.RequestCounterAttack(_idamageable.LastAttacker);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.Hitted)
            {
                int chanceValue = Random.Range(0, 101);

                if (chanceValue <= _data.CostValue)
                {
                    ExcuteAbility();
                }
            }
        }
    }
}