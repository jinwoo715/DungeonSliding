using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class CounterAttack : RuleAbility
    {
        ICombatant _combatant;

        public CounterAttack(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            if (Host.TryGet<ICombatant>(out var service))
            {
                _combatant = service;
            }
        }

        public override void ExcuteAbility()
        {
            _combatant.RequestCounterAttack(_combatant.LastAttacker);
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