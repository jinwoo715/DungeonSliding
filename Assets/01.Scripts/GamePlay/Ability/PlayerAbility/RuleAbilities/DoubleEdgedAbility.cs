using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class DoubleEdgedAbility : RuleAbilityBase
    {
        IAttackable _attackable;
        IDamageable _damageable;
        public DoubleEdgedAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<IAttackable>(ref _attackable);
            BindService<IDamageable>(ref _damageable);
        }
    }
}