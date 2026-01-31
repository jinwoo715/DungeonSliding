using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ExtraAttackChance : RuleAbility
    {
        INextAttackEnhancer _service;

        public ExtraAttackChance(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            if (Host.TryGet<INextAttackEnhancer>(out var service))
            {
                _service = service;
            }
        }

        public override void ExcuteAbility()
        {
            _service.AddEnhance(ENextAttackEnhanceType.ExtraAttack,1);
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