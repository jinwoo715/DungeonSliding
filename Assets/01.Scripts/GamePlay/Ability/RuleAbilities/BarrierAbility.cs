using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : RuleAbility
    {
        IBarrierable _barrierable;
        public BarrierAbility(RuleAbilitySOData data, IAbilityHost host) : base(data, host) 
        {
            BindService<IBarrierable>(ref _barrierable);
        }

        public override void ExcuteAbility()
        {
            _barrierable.GainBarrier();
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnEnterRoom)
            {
                ExcuteAbility();
            }
        }
    }
}