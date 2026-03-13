using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ReviveAbility : RuleAbilityBase
    {
        private int isReviveCount = 0;
        public ReviveAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            throw new System.NotImplementedException();
        }
    }
}
