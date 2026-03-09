using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class WallBounceAbility : AbilityBase
    {
        public IMoveable _moveable;
        public ICombatant _combatant;
        public WallBounceAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility()
        {
            EDirectionType bounceDir = GameUtil.ReverseDirection(_combatant.Rotate.Direction);
            _moveable.KnockBack(bounceDir);
        }

        public override void ProcTrigger(EGameEventTrigger triggerType)
        {
            if (_moveable.SlideTileCount() == 1 && _moveable.SlideResultType == ESlideResultType.Stop)
            {
                ExcuteAbility();
            }
        }

        protected override void BindService()
        {
            BindService<IMoveable>(ref _moveable);
            BindService<ICombatant>(ref _combatant);
        }
    }
}