using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class RerollPlusAbility : AbilityBase
    {
        IRerollService _rerollService;

        public RerollPlusAbility(RuleAbilityData data, AbilityHost host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility()
        {
            _rerollService.AddReroll();
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
        }

        protected override void BindService()
        {
            BindService<IRerollService>(ref _rerollService);
        }
    }
}
