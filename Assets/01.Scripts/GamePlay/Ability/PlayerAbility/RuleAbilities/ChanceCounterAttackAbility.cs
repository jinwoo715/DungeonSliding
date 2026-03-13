using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ChanceCounterAttackAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackResultPayload>
    {
        IAttackRequester _attackRequester;
        AttackResultPayload _payload;

        public ChanceCounterAttackAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_payload.IsCounterAttack) yield break;

            if(IsCheckChanceSuccess(_data.P1))
            {
                _attackRequester.RequestCounterAttack(_payload.Attacker);
            }

            yield break;
        }

        public void ReceivePayload(AttackResultPayload payload)
        {
            _payload = payload;
        }

        protected override void BindService()
        {
            BindService(ref _attackRequester);
        }
    }
    public class ChanceExtraAttackAbility : RuleAbilityBase
    {
        INextAttackEnhancer _nextAttackEnhancer;
        public ChanceExtraAttackAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (IsCheckChanceSuccess(_data.P1))
            {
                _nextAttackEnhancer.AddNextAttackCount((Mathf.RoundToInt(_data.P2)));
            }
            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _nextAttackEnhancer);
        }
    }
    public class DistanceExtraDamageAbility : RuleAbilityBase
    {
        IRouteService _routeService;
        INextAttackEnhancer _nextAttackEnhancer;

        public DistanceExtraDamageAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int tileCount = _routeService.LastMoveTileCount;
            int addDamage = tileCount * Mathf.RoundToInt(_data.P1);
            _nextAttackEnhancer.AddNextAttackDamage(addDamage);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _routeService);
            BindService(ref _nextAttackEnhancer);
        }
    }
    public class SurrondEnemyAddDamageAbility : RuleAbilityBase
    {
        ICombatantSensor _combatantSensor;
        IStatModifier _statModifier;
        IReadOnlyTilePosition _tile;
        int value = 0;

        public SurrondEnemyAddDamageAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int enemyCount = _combatantSensor.GetNearEnemyCount(_tile.TilePosition);
            int addDamage = enemyCount * (Mathf.RoundToInt(_data.P1));

            StatModifierContext context = new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Add, -value);

            _statModifier.ModifyStat(context);

            value = addDamage;

            context.SetValue(addDamage);

            _statModifier.ModifyStat(context);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _combatantSensor);
            BindService(ref _statModifier);
        }
    }
    public class GlassCannonAbility : RuleAbilityBase
    {
        IStatModifier _statModifier;
        public GlassCannonAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.DamageDealtMultiplier, EApplyStatType.Add, _data.P1));
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.DamageTakeMultiplier, EApplyStatType.Add, _data.P2));

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
        }
    }
    public class FadingStrengthAbility : RuleAbilityBase
    {
        IStatModifier _statModifier;
        bool _isInit = false;
        float _remainValue = 0;

        public FadingStrengthAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_isInit)
            {
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, _data.P1));
                _isInit = true;
            }
            else
            {
                if (_remainValue >= _data.P1)
                    yield break;

                _remainValue += _data.P2;
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, -_data.P2));
            }

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
        }
    }



    //Recovery
    public class RecoveryEffect
    {
        private IStatModifier _statModifier;
        private ECreatureStatType _recoveryStatType = ECreatureStatType.None;

        public RecoveryEffect(ECreatureStatType statType, IStatModifier statModifier)
        {
            _statModifier = statModifier;
            _recoveryStatType = statType;
        }

        public void Recovery(int value)
        {
            _statModifier.ModifyStat(new StatModifierContext(_recoveryStatType, EApplyStatType.Add, value));
        }
    }
    public class BloodDrainAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackResultPayload>
    {
        IStatModifier _statModifier;
        private RecoveryEffect _recoveryEffect;
        int _appliedDamage = 0;

        public BloodDrainAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int recoveryValue = Mathf.RoundToInt(_appliedDamage * _data.P1);
            _recoveryEffect.Recovery(recoveryValue);
            yield break;
        }

        public void ReceivePayload(AttackResultPayload payload)
        {
            _appliedDamage = payload.Damage;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            _recoveryEffect = new RecoveryEffect(ECreatureStatType.CurrentHP, _statModifier);
        }
    }
    public class VitalAbsorbAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackResultPayload>
    {
        IStatModifier _statModifier;
        private RecoveryEffect _recoveryEffect;
        int _appliedDamage = 0;

        public VitalAbsorbAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int recoveryValue = Mathf.RoundToInt(_appliedDamage * _data.P1);
            _recoveryEffect.Recovery(recoveryValue);

            yield break;
        }

        public void ReceivePayload(AttackResultPayload payload)
        {
            _appliedDamage = payload.Damage;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            _recoveryEffect = new RecoveryEffect(ECreatureStatType.CurrentMoveCount, _statModifier);
        }
    }

    //Revive
    public class EmergencyConvertMoveToHPAbility : RuleAbilityBase
    {
        IStatModifier _statModifier;
        IStatReadOnly _statReadOnly;
        public EmergencyConvertMoveToHPAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int remainMove = _statReadOnly.Get(ECreatureStatType.CurrentMoveCount);

            if (remainMove > _data.P1)
            {
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, -_data.P1));
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, _data.P2));
            }

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            BindService(ref _statReadOnly);
        }
    }
    public class EmergencyConvertHPToMoveAbility : RuleAbilityBase
    {
        IStatModifier _statModifier;
        IStatReadOnly _statReadOnly;
        public EmergencyConvertHPToMoveAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int remainHP = _statReadOnly.Get(ECreatureStatType.CurrentHP);

            if(remainHP > _data.P1)
            {
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -_data.P1));
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, _data.P2));
            }

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            BindService(ref _statReadOnly);
        }
    }

    //불사조의 의지
    public class PhoenixWillAbility : RuleAbilityBase
    {
        private IStatModifier _statModifier;
        private IStatReadOnly _statReadOnly;
        private int reviveCount = 0;

        public PhoenixWillAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (reviveCount <= 0) yield break;

            reviveCount--;

            int maxHP = _statReadOnly.Get(ECreatureStatType.MaxHp);
            int maxMove = _statReadOnly.Get(ECreatureStatType.MaxMoveCount);

            int percentHP = Mathf.RoundToInt(maxHP * _data.P2);
            int percentMove = Mathf.RoundToInt(maxMove * _data.P2);

            int currentHP = _statReadOnly.Get(ECreatureStatType.CurrentHP);
            int currentMove = _statReadOnly.Get(ECreatureStatType.CurrentMoveCount);

            if(currentHP < percentHP)
            {
                int diff = percentHP - currentHP;
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, diff));
            }

            if (currentMove < percentMove)
            {
                int diff = percentMove - currentMove;
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, diff));
            }

            yield break;
        }
        protected override void InitData()
        {
            reviveCount = Mathf.RoundToInt(_data.P1);
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            BindService(ref _statReadOnly);
        }
    }

    //처형 선고
    public class FinishingBlowAbility : RuleAbilityBase
    {
        IAttackable _attackable;

        public FinishingBlowAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int excutionRatio = Mathf.RoundToInt(_data.P1);
            _attackable.AddStatusEffect(EStatusEffectType.Execution, excutionRatio);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _attackable);
        }
    }

    //일대일의 긍지
    public class IsolationAbility : RuleAbilityBase
    {
        ICombatantSensor _sensor;
        ITileObject _ownerPosition;
        IStatModifier _statModifier;
        float value = 0;
        
        public IsolationAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int nearEnemyCount = _sensor.GetNearEnemyCount(_ownerPosition.TilePosition);

            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, -value));

            if(nearEnemyCount == 1)
            {
                value = _data.P1;
                _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, value));
            }
            else
            {
                value = 0;
            }

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
            BindService(ref _ownerPosition);
        }
    }

    //빈틈 포착
    public class FlankCriticalAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        ICombatant _attacker;
        ICombatant _target;
        INextAttackEnhancer _nextAttackEnhancer;
        public FlankCriticalAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            bool isFlankAttack = DirectionUtility.IsSideAttack(_attacker, _target);

            if (isFlankAttack)
            {
                _nextAttackEnhancer.GuaranteedCritical();
            }
            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            _target = payload.Target;
        }

        protected override void BindService()
        {
            BindService(ref _attacker);
            BindService(ref _nextAttackEnhancer);
        }
    }

    //격차 이용
    public class WeakContemptAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        IStatReadOnly _statReadOnly;
        IStatReadOnly _targetStat;
        INextAttackEnhancer _nextAttackEnhancer;

        public WeakContemptAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int ownerDamage = _statReadOnly.Get(ECreatureStatType.Damage);
            int targetDamage = _targetStat.Get(ECreatureStatType.Damage);

            int diff = ownerDamage - targetDamage;

            if (diff > 0)
                _nextAttackEnhancer.AddNextAttackDamage(diff);

            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            _targetStat = payload.Target.StatReadOnly;
        }

        protected override void BindService()
        {
            BindService(ref _statReadOnly);
        }
    }

    //한놈만 패
    public class FocusStrikeAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        INextAttackEnhancer _nextAttackEnhancer;
        ICombatant _lastTarget;
        bool _isSameTarget = false;
        float _stackDamage = 0;
        public FocusStrikeAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if(_isSameTarget)
            {
                _stackDamage += _data.P1;

                _nextAttackEnhancer.AddNextAttackDamageMulti(_stackDamage);
            }

            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            if (_lastTarget != null && _lastTarget == payload.Target)
            {
                _isSameTarget = true;
            }
            else
            {
                _isSameTarget = false;
                _stackDamage = 0;
            }
        }

        protected override void BindService()
        {
            BindService(ref _nextAttackEnhancer);
        }
    }
    public class VitalStrikeAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        IStatReadOnly _targetStat;
        INextAttackEnhancer _nextAttackEnhancer;

        public VitalStrikeAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int currentTargetHP = _targetStat.Get(ECreatureStatType.CurrentHP);
            int maxTargetHP = _targetStat.Get(ECreatureStatType.MaxHp);

            int ratio = Mathf.RoundToInt(currentTargetHP / maxTargetHP);

            if(ratio <= _data.P1)
            {
                _nextAttackEnhancer.AddNextAttackDamageMulti(_data.P2);
            }

            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            _targetStat = payload.Target.StatReadOnly;
        }

        protected override void BindService()
        {
            BindService(ref _nextAttackEnhancer);
        }
    }
}