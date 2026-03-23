using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Enemy
{
    public class HeavyGravity : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        public HeavyGravity(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }
        public override IEnumerator Execute(AbilityArgs args)
        {
            Debug.Log($"{args.CreatureTrigger}");

            if (args.CreatureTrigger == _data.ReleaseCreatureTrigger)
                ReleaseAbility();

            if(args.CreatureTrigger == _data.CretureTriggerType)
                _moveRule.AddMoveCost(Mathf.RoundToInt(P1));

            yield break;
        }
        public override void ReleaseAbility()
        {
            Debug.Log("Recovery");
            _moveRule.AddMoveCost(-Mathf.RoundToInt(P1));
        }

        protected override void BindService()
        {
            BindService(ref _moveRule);
        }
    }
    public class MoveBanToDirection : EnemyAbilityBase
    {
        IMoveRule _moveRule;

        public MoveBanToDirection(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (args.CreatureTrigger == _data.ReleaseCreatureTrigger)
                ReleaseAbility();

            if (args.CreatureTrigger == _data.CretureTriggerType)
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
