using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BarrierAbility : RuleAbilityBase
    {
        IBarrierable _barrierable;
        public BarrierAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<IBarrierable>(ref _barrierable);
        }
    }
}