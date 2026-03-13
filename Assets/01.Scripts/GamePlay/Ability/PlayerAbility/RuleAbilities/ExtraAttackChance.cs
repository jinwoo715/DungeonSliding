using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ExtraAttackChance : RuleAbilityBase
    {
        INextAttackEnhancer _service;
        IMoveable _moveable;
        public ExtraAttackChance(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<INextAttackEnhancer>(ref _service);
            BindService(ref _moveable);

        }
    }
}