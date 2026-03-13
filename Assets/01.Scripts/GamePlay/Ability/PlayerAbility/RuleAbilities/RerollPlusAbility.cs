using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class RerollPlusAbility : RuleAbilityBase
    {
        IRerollService _rerollService;

        public RerollPlusAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<IRerollService>(ref _rerollService);
        }
    }
}
