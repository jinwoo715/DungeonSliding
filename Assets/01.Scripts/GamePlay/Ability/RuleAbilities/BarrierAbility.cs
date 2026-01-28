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

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.EnterRoom)
            {
                ExcuteAbility();
            }
        }
    }
}