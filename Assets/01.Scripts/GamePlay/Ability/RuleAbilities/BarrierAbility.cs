using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : RuleAbility
    {
        private ICombatant _combatant;

        public BarrierAbility(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            Entity.GainBarrier();
            _combatant.GainBarrier();
        }

        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.EnterRoom)
            {
                ExcuteAbility();
            }
        }
    }
}