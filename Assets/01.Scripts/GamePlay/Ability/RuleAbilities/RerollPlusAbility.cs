using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class RerollPlusAbility : RuleAbility
    {
        IRerollService _rerollService;

        public RerollPlusAbility(RuleAbilityData data, IAbilityHost host) : base(data, host)
        {
            BindService<IRerollService>(ref _rerollService);
        }

        public override void ExcuteAbility()
        {
            _rerollService.AddReroll();
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}
