using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ExtraAttackChance : RuleAbility
    {
        ICombatant _combatant;

        public ExtraAttackChance(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            _combatant.CurrentAttackBuff.AddExtraAttack();
        }

        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.MoveEnd)
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