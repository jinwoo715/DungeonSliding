using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideAmplifierAbility : RuleAbility
    {
        ICombatant _combatant;
        IMoveable _moveable;
        public SlideAmplifierAbility(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            int addDamage = _moveable.SlideTileCount();
            _combatant.CurrentAttackBuff.AddDamage(addDamage);
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