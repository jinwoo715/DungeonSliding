using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class WallBounceAbility : RuleAbility
    {
        public IMoveable _moveable;

        public WallBounceAbility(RuleAbilityData data, IAbilityHost host) : base(data, host)
        {
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            EDirectionType bounceDir = GameUtil.ReverseDirection(_moveable.MoveDir);
            _moveable.MoveStep(bounceDir);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (_moveable.SlideTileCount() == 1 && _moveable.SlideResultType == ESlideResultType.EnemyStop)
            {
                ExcuteAbility();
            }
        }
    }
}