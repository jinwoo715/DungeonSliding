using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using System.Collections;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class WallBounceAbility : RuleAbilityBase
    {
        public IMoveable _moveable;
        public ICombatant _combatant;
        public WallBounceAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<IMoveable>(ref _moveable);
            BindService<ICombatant>(ref _combatant);
        }
    }
}