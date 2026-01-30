using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ExtraAttackChance : RuleAbility
    {
        ICombatant _combatant;

        public ExtraAttackChance(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            if (Host.TryGet<ICombatant>(out var service))
            {
                _combatant = service;
            }
        }

        public override void ExcuteAbility()
        {
            _combatant.AttackBuff.AddExtraAttack();
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.MoveEnd)
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