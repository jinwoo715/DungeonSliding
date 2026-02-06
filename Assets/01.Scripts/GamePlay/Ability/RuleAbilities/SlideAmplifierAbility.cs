using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideAmplifierAbility : RuleAbility
    {
        INextAttackEnhancer _nextAttackEnhancer;
        IMoveable _moveable;
        public SlideAmplifierAbility(RuleAbilitySOData data, IAbilityHost host) : base(data, host) 
        {
            BindService<INextAttackEnhancer>(ref _nextAttackEnhancer);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            int addDamage = _moveable.SlideTileCount();
            _nextAttackEnhancer.AddEnhance(ENextAttackType.Add, addDamage);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}