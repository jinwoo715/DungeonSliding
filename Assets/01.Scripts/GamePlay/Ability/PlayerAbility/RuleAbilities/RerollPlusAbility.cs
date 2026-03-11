using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class RerollPlusAbility : RuleAbilityBase
    {
        IRerollService _rerollService;

        public RerollPlusAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility()
        {
            _rerollService.AddReroll();
        }

        public override void ProcTrigger(EGameEventTrigger triggerType)
        {
        }

        protected override void BindService()
        {
            BindService<IRerollService>(ref _rerollService);
        }
    }
}
