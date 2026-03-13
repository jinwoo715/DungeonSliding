using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertHpToMoveCount : RuleAbilityBase
    {
        IStatReadOnly _statReadOnly;
        IPlayerStatModifier _statModifier;
        IMoveable _moveable;
        public ConvertHpToMoveCount(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService(ref _statReadOnly);
            BindService(ref _statModifier);
            BindService<IMoveable>(ref _moveable);
        }
    }
}
