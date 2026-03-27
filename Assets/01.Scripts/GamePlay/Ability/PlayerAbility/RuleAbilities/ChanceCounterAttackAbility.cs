using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    //TODO 카운터 발동 연출 필요해보임
    public class ChanceCounterAttackAbility : RuleAbilityBase, IAbilityPayloadReceiver<HitResultPayload>
    {
        IAttackRequester _attackRequester;
        HitResultPayload _payload;

        public ChanceCounterAttackAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_payload.IsCounterAttack) yield break;

            if(Chance.IsChanceSuccess(_data.P1))
            {
                _attackRequester.RequestCounterAttack(_payload.Attacker);
            }

            yield break;
        }

        public void ReceivePayload(HitResultPayload payload)
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
            if (Chance.IsChanceSuccess(_data.P1))
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
        INextAttackEnhancer _nextAttackEnhancer;
        ITileObject _tile;

        public SurrondEnemyAddDamageAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int enemyCount = _combatantSensor.GetNearEnemyCount(_tile.TilePosition);
            int addDamage = enemyCount * (Mathf.RoundToInt(_data.P1));

            _nextAttackEnhancer.AddNextAttackDamage(addDamage);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _tile);
            BindService(ref _combatantSensor);
            BindService(ref _nextAttackEnhancer);
        }
    }

    //양날의 검
    //TODO 주는 피해에 대한 표시를 해줘야 하나?
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
        float _remainValue = 0;

        public FadingStrengthAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_remainValue >= _data.P1)
                yield break;

            float multiplier = _data.P2 / 100;

            _remainValue += _data.P2;
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, -multiplier));

            yield break;
        }

        protected override void InitData()
        {
            float multiplier = _data.P1 / 100;
            _statModifier.ModifyStat(new StatModifierContext(ECreatureStatType.Damage, EApplyStatType.Multiple, multiplier));
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

    //생명 흡수
    public class BloodDrainAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackResultPayLoad>
    {
        IStatModifier _statModifier;
        private RecoveryEffect _recoveryEffect;
        int _appliedDamage = 0;

        public BloodDrainAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            float ratio = _data.P1 * 0.01f;
            int recoveryValue = Mathf.CeilToInt(_appliedDamage * ratio);
            _recoveryEffect.Recovery(recoveryValue);
            yield break;
        }
        public void ReceivePayload(AttackResultPayLoad payload)
        {
            _appliedDamage = payload.AppliedDamage;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            _recoveryEffect = new RecoveryEffect(ECreatureStatType.CurrentHP, _statModifier);
        }
    }

    //기력 흡수
    public class VitalAbsorbAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackResultPayLoad>
    {
        IStatModifier _statModifier;
        private RecoveryEffect _recoveryEffect;
        int _appliedDamage = 0;

        public VitalAbsorbAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            float ratio = _data.P1 * 0.01f;
            int recoveryValue = Mathf.CeilToInt(_appliedDamage * ratio);
            _recoveryEffect.Recovery(recoveryValue);

            yield break;
        }

        public void ReceivePayload(AttackResultPayLoad payload)
        {
            _appliedDamage = payload.AppliedDamage;
        }

        protected override void BindService()
        {
            BindService(ref _statModifier);
            _recoveryEffect = new RecoveryEffect(ECreatureStatType.CurrentMoveCount, _statModifier);
        }
    }

    //TODO 연출 고민
    #region 부활 메커니즘
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

            float recoveryRatio = _data.P2 / 100;
            
            int percentHP = Mathf.CeilToInt(maxHP * recoveryRatio);
            int percentMove = Mathf.CeilToInt(maxMove * recoveryRatio);

            int currentHP = _statReadOnly.Get(ECreatureStatType.CurrentHP);
            int currentMove = _statReadOnly.Get(ECreatureStatType.CurrentMoveCount);

            Debug.Log($"{maxHP} : {percentHP} : {currentHP}");
            Debug.Log($"{maxMove} : {percentMove}: {currentMove}");

            if (currentHP < percentHP)
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
    #endregion

    //처형 선고
    public class FinishingBlowAbility : RuleAbilityBase
    {
        IAttackable _attackable;

        public FinishingBlowAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int excutionRatio = Mathf.CeilToInt(_data.P1);
            _attackable.AddStatusEffect(ECreatureStatus.Execution, excutionRatio);

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
        INextAttackEnhancer _nextAttackEnhancer;
        
        public IsolationAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int nearEnemyCount = _sensor.GetNearEnemyCount(_ownerPosition.TilePosition);

            if(nearEnemyCount == 1)
            {
                float multiplier = _data.P1 * 0.01f;
                _nextAttackEnhancer.AddNextAttackDamageMulti(multiplier);
            }

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
            BindService(ref _ownerPosition);
            BindService(ref _nextAttackEnhancer);
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

    //약자 멸시
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
            BindService(ref _nextAttackEnhancer);
        }
    }

    //한놈만 패
    public class FocusStrikeAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        INextAttackEnhancer _nextAttackEnhancer;
        ICombatant _lastTarget = null;
        bool _isSameTarget = false;
        int _multiplyStack = 0;
        public FocusStrikeAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if(_isSameTarget)
            {
                float ratio = _data.P1 * 0.01f;

                float addMultiply = _multiplyStack * ratio;
                _nextAttackEnhancer.AddNextAttackDamageMulti(addMultiply);
            }

            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            if (_lastTarget != null && _lastTarget == payload.Target)
            {
                _isSameTarget = true;
                _multiplyStack++;
            }
            else
            {
                _lastTarget = payload.Target;
                _isSameTarget = false;
                _multiplyStack = 0;
            }
        }

        protected override void BindService()
        {
            BindService(ref _nextAttackEnhancer);
        }
    }

    //급소 노리기
    public class VitalStrikeAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        IStatReadOnly _targetStat;
        INextAttackEnhancer _nextAttackEnhancer;

        public VitalStrikeAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            int currentTargetHP = _targetStat.Get(ECreatureStatType.CurrentHP);
            int maxTargetHP = _targetStat.Get(ECreatureStatType.MaxHp);

            float pivotRatio = _data.P1 * 0.01f;
            int pivotHP = Mathf.CeilToInt(maxTargetHP * pivotRatio);

            if(currentTargetHP <= pivotHP)
            {
                float multiple = _data.P2 * 0.01f;
                _nextAttackEnhancer.AddNextAttackDamageMulti(multiple);
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

    //속박 일격
    //TODO 속박 연출 필요해보임
    public class ShackleStrikeAbility : RuleAbilityBase, IAbilityPayloadReceiver<AttackPreparePayLoad>
    {
        IAttackable _attackable;
        HashSet<ICombatant> _shackedList = new HashSet<ICombatant>();
        ICombatant _attackTarget;

        public ShackleStrikeAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnClearStage, ReleaseAbility);
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (IsApplyShackle())
            {
                _shackedList.Add(_attackTarget);
                _attackable.AddStatusEffect(ECreatureStatus.Bind, 1);
                _attackTarget = null;
            }

            yield break;
        }

        public void ReceivePayload(AttackPreparePayLoad payload)
        {
            _attackTarget = payload.Target;
        }

        private bool IsApplyShackle()
        {
            if (_attackTarget == null) return false;

            return !_shackedList.Contains(_attackTarget);
        }

        public override void ReleaseAbility()
        {
            _shackedList.Clear();
        }

        protected override void BindService()
        {
            BindService(ref _attackable);
        }
    }

    public class RerollPlusAbility : RuleAbilityBase
    {
        IRerollService _rerollService;
        public RerollPlusAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _rerollService.AddReroll(1);
            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _rerollService);
        }
    }

    //뜻밖의 수확
    public class GetExtraAbility : RuleAbilityBase
    {
        IAbilityRandomGetter _abilityRandomGetter;

        public GetExtraAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _abilityRandomGetter.GetRandomAbility(2);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _abilityRandomGetter);
        }
    }

    public class SpellShieldAbility : RuleAbilityBase
    {
        IStatusModifier _statusModifier;
        public SpellShieldAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            _statusModifier.ApplyStatus(Entities.ECreatureStatus.Barrier, 1);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _statusModifier);
        }
    }

    public class WallBounceAbility : RuleAbilityBase
    {
        public IRotateObject _rotate;
        public IMoveable _moveable;

        public WallBounceAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            EDirectionType backDir = DirectionUtility.ReverseDirection(_rotate.Direction);
            _moveable.KnockBack(backDir);

            yield break;
        }

        protected override void BindService()
        {
            BindService(ref _moveable);
            BindService(ref _rotate);
        }
    }

    //그림자 숨기
    public class HideShadowAbility : RuleAbilityBase, IAbilityPayloadReceiver<BattleResultPayLoad>
    {
        IStatusModifier _statusModifier;
        IMoveable _moveable;
        int _nonBattleCount = 0;

        bool _isHideMode = false;
        bool _isCombatted = false;

        public HideShadowAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if(args.CreatureTrigger == ECreatureTrigger.OnRegisterAttack)
            {
                if (_isHideMode == true)
                {
                    _statusModifier.RemoveStatus(ECreatureStatus.Hide);
                    _nonBattleCount = 0;
                    _isHideMode = false;

                    yield break;
                }
            }

            if (_moveable.SlideTileCount() < 1)
                yield break;

            if (!_isHideMode)
            {
                if (!_isCombatted)
                {
                    _nonBattleCount++;

                    if (_nonBattleCount >= _data.P1)
                    {
                        _isHideMode = true;
                        _statusModifier.ApplyStatus(Entities.ECreatureStatus.Hide, 1);
                    }
                }
                else
                {
                    _nonBattleCount = 0;
                }
            }
            Debug.Log(_nonBattleCount);
            yield break;
        }

        public void ReceivePayload(BattleResultPayLoad payload)
        {
            _isCombatted = payload.IsCombatted;
            Debug.Log($"BattleResultPayLoad {_isCombatted}");
        }

        protected override void BindService()
        {
            BindService(ref _statusModifier);
            BindService(ref _moveable);
        }
    }

}