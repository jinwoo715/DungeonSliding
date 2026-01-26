using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class CounterAttack : RuleAbility
    {
        ICombatant _combatant;

        public CounterAttack(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            _combatant.RequestCounterAttack(_combatant.LastAttacker);
        }

        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.Hitted)
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