using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.MoveRule
{
    public class HeavyGravityAbility : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        public HeavyGravityAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }
        public override IEnumerator Execute(AbilityArgs args)
        {
            _moveRule.AddMoveCost(Mathf.RoundToInt(P1));

            yield break;
        }
        public override void ReleaseAbility()
        {
            _moveRule.AddMoveCost(-Mathf.RoundToInt(P1));
        }

        protected override void BindService()
        {
            BindService(ref _moveRule);
        }
    }
    public class FacingMoveBanAbility : EnemyAbilityBase
    {
        IMoveRule _moveRule;

        public FacingMoveBanAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _moveRule.SetMoveBanDirection(_owner.Rotate.Direction);
            yield break;
        }
        public override void ReleaseAbility()
        {
            _moveRule.SetMoveBanDirection(EDirectionType.None);
        }
        protected override void BindService()
        {
            BindService(ref _moveRule);
        }
    }
}
