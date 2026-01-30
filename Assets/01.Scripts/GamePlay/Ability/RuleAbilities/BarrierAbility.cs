using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : RuleAbility
    {
        public BarrierAbility(RuleAbilityData data, IAbilityHost host) : base(data, host) { }

        public override void ExcuteAbility()
        {
            if (Host.TryGet<IDamageable>(out var service))
            {
                service.GainBarrier();
            }
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.EnterRoom)
            {
                ExcuteAbility();
            }
        }
    }
}