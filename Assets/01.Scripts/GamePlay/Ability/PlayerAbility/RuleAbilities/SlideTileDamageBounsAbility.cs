using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideTileDamageBounsAbility : RuleAbilityBase
    {
        INextAttackEnhancer _nextAttackEnhancer;
        IMoveable _moveable;
        public SlideTileDamageBounsAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<INextAttackEnhancer>(ref _nextAttackEnhancer);
            BindService<IMoveable>(ref _moveable);
        }
    }
}