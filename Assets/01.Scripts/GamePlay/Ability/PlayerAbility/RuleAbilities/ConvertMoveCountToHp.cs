using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System.Collections;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertMoveCountToHp : RuleAbilityBase
    {
        Combat.IPlayerStatModifier _playerStatModifier;
        Stats.IStatReadOnly _statReadOnly;
        public ConvertMoveCountToHp(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService(ref _playerStatModifier);
            BindService(ref _statReadOnly);
        }
    }
}