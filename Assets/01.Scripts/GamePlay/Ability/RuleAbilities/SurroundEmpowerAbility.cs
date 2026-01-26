using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SurroundEmpowerAbility : RuleAbility
    {
        private ICombatantSensor _sensor;
        private ICombatant _combatant;
        public SurroundEmpowerAbility(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            int count = _sensor.GetNearCambatantCount(_combatant);

            _combatant.CurrentAttackBuff.AddDamage(count);
        }

        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.MoveEnd)
            {
                ExcuteAbility();
            }
        }
    }
}