using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : RuleAbilityBase
    {
        IBarrierable _barrierable;
        public BarrierAbility(RuleAbilityData data, AbilityHost host) : base(data, host) 
        {
        }

        public override void ExcuteAbility()
        {
            _barrierable.GainBarrier();
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == _data.TriggerType)
            {
                ExcuteAbility();
            }
        }

        protected override void BindService()
        {
            BindService<IBarrierable>(ref _barrierable);
        }
    }
}