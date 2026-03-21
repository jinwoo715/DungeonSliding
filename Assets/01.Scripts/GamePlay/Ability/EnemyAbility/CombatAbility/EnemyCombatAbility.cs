using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Combat
{
    public class ChanceCounterAbility : EnemyAbilityBase, IAbilityPayloadReceiver<HitResultPayload>
    {
        IAttackRequester _attackRequester;
        HitResultPayload _payLoad;
        public ChanceCounterAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_payLoad.IsCounterAttack)
                yield break;

            if (Chance.IsChanceSuccess(P1))
            {
                _attackRequester.RequestCounterAttack(_payLoad.Attacker);
            }

            yield break;
        }

        public void ReceivePayload(HitResultPayload payload)
        {
            _payLoad = payload;
        }

        protected override void BindService()
        {
            _attackRequester = _owner.AttackRequester;
        }
    }
    public class DefenceFrontAttackAbility : EnemyAbilityBase, IAbilityPayloadReceiver<TakeAttackPayLoad>
    {
        TakeAttackPayLoad _payload;
        IStatusModifier _statusModifier;
        public DefenceFrontAttackAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if(DirectionUtility.IsFacingAttack(_payload.Attacker, _owner))
            {
                _statusModifier.ApplyStatus(Entities.ECreatureStatus.Barrier, 1);
            }

            yield break;
        }

        public void ReceivePayload(TakeAttackPayLoad payload)
        {
            _payload = payload;
        }

        protected override void BindService()
        {
            _statusModifier = _owner.StatusModifier;
        }
    }
    public class KnockBackAttackAbility : EnemyAbilityBase
    {
        IAttackable _attackable;
        public KnockBackAttackAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _attackable.AddStatusEffect(ECreatureStatus.Knockback, 1);
            yield break;
        }

        protected override void BindService()
        {
            _attackable = _owner;
        }
    }
}
