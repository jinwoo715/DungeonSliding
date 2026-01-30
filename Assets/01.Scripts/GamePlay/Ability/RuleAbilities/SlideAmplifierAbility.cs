using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideAmplifierAbility : RuleAbility
    {
        ICombatant _combatant;
        IMoveable _moveable;
        public SlideAmplifierAbility(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            BindService<ICombatant>(ref _combatant);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            int addDamage = _moveable.SlideTileCount();
            _combatant.AttackBuff.AddDamage(addDamage);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}