using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class DoubleEdgedAbility : RuleAbility
    {
        ICombatant _combatant;
        public DoubleEdgedAbility(RuleAbilityData data, Player player) : base(data, player)
        {
            _combatant.DamageDealtMultiplier = _data.GainValue;
            _combatant.DamageTakenMultiplier = _data.GainValue;
        }

        public override void ExcuteAbility() { }
        public override void ProcTrigger(EAbilityTriggerType triggerType) { }
    }
}