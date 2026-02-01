using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class DoubleEdgedAbility : RuleAbility
    {
        ICombatant _combatant;
        public DoubleEdgedAbility(RuleAbilityData data, IAbilityHost host) : base(data, host)
        {
            BindService<ICombatant>(ref _combatant);

            _combatant.AddDamageDealtMultiplier(_data.GainValue);
            _combatant.AddDamageTakenMultiplier(_data.GainValue);
        }

        public override void ExcuteAbility() { }
        public override void ProcTrigger(EGameTriggerType triggerType) { }
    }
}