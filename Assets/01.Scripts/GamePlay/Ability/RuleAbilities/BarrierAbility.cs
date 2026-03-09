using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : AbilityBase
    {
        IBarrierable _barrierable;
        public BarrierAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
        }

        public override void ExcuteAbility()
        {
            _barrierable.GainBarrier();
        }

        public override void ProcTrigger(EGameEventTrigger triggerType)
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