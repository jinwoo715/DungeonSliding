using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class DoubleEdgedAbility : AbilityBase
    {
        IAttackable _attackable;
        IDamageable _damageable;
        public DoubleEdgedAbility(RuleAbilityData data, AbilityHost host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility() 
        {
            _attackable.AddDamageDealtMultiplier(_data.P1 * 0.01f);
            _damageable.AddDamageTakenMultiplier(_data.P2 * 0.01f);
        }
        public override void ProcTrigger(EGameTriggerType triggerType) { }

        protected override void BindService()
        {
            BindService<IAttackable>(ref _attackable);
            BindService<IDamageable>(ref _damageable);
        }
    }
}