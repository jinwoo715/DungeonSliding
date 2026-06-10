using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Enemy
{
    public class HeavyGravity : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        bool _isApplied;

        public HeavyGravity(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }
        public override IEnumerator Execute(AbilityArgs args)
        {
            Debug.Log($"{args.CreatureTrigger}");

            if (args.CreatureTrigger == _data.ReleaseCreatureTrigger)
                ReleaseAbility();

            if(args.CreatureTrigger == _data.CretureTriggerType && !_isApplied)
            {
                _moveRule.AddMoveCost(Mathf.RoundToInt(P1));
                _isApplied = true;
            }

            yield break;
        }
        public override void ReleaseAbility()
        {
            if (!_isApplied)
                return;

            Debug.Log("Recovery");
            _moveRule.AddMoveCost(-Mathf.RoundToInt(P1));
            _isApplied = false;
        }

        protected override void BindService()
        {
            BindService(ref _moveRule);
        }
    }
    public class MoveBanToDirection : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        Action _onRotateEnd;
        bool _isReleased;

        //TurnEnd -> RotateEnd

        public MoveBanToDirection(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section)
        {
            _onRotateEnd = ApplyMoveBanDirection;
            ApplyMoveBanDirection();
            owner.Rotate.OnRotateEnd += _onRotateEnd;
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (args.CreatureTrigger == _data.ReleaseCreatureTrigger)
                ReleaseAbility();

//            if (args.CreatureTrigger == _data.CretureTriggerType)
//                _moveRule.SetMoveBanDirection(_owner.Rotate.Direction);

            yield break;
        }
        public override void ReleaseAbility()
        {
            if (_isReleased)
                return;

            _isReleased = true;
            _owner.Rotate.OnRotateEnd -= _onRotateEnd;
            _moveRule.SetMoveBanDirection(EDirectionType.None);
        }

        private void ApplyMoveBanDirection()
        {
            _moveRule.SetMoveBanDirection(_owner.Rotate.Direction);
        }

        protected override void BindService()
        {
            BindService(ref _moveRule);
        }
    }
}
