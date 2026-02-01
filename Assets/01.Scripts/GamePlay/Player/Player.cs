using DG.Tweening;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using JW.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class StatValue
    {
        public int Base;      // 기본값
        public int Add;       // 덧셈 보너스(+)
        public float Mul;     // 곱셈 보너스(×), 기본 1f

        public Dictionary<EPlayerStat, float> RatioValueByStat;

        public StatValue(int baseValue)
        {
            Base = baseValue;
            Add = 0;
            Mul = 1;

            RatioValueByStat = new();
        }

        public void AddRatio(EPlayerStat stat, float value)
        {
            if (!RatioValueByStat.ContainsKey(stat))
                RatioValueByStat.Add(stat, 0);

            RatioValueByStat[stat] += value;
        }

        public int Final(IPlayerStatReadOnly StatReadOnly) 
        {
            float addRatioValue = 0;
            foreach (var item in RatioValueByStat)
            {
                addRatioValue += StatReadOnly.Get(item.Key) * item.Value;
            }

            return Mathf.FloorToInt((Base + Add + addRatioValue) * Mul);
        }

        public void ResetBonus()
        {
            Add = 0;
            Mul = 1f;
            RatioValueByStat.Clear();
        }
    }

    public class Player : Creature, IMoveable, IAbilityHost, INextAttackEnhancer, IPlayerStatModifier, IPlayerStatReadOnly
    {
        private RoutePlanner _route = new RoutePlanner();
        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType => throw new NotImplementedException();
        public EDirectionType MoveDir => throw new NotImplementedException();
        public Action<EPlayerStat> OnStatChanged { get; set; }
        public event Func<Tile, EDirectionType, ETileEnterType, MoveContext> GetMoveContextFunc;
        public event Action FinishSlideEvent;
        public event Action LevelUpEvent;

        public IGameModeChanger _gameModeChanger;

        private int _level = 1;
        private int _levelUpXp = 0;
        private int _currentXp = 0;
        private bool _isKnockbacking = false;

        private NextAttackBuff _nextAttackBuff;

        private StatValue MaxHp;
        private StatValue Damage;
        private StatValue MaxMoveCount;
        private int _currentMoveCount;

        public Action OnDeathEvent;

        //데미지를 받았을 때,
        //Stat이 변경됐을 때,
        //다음 공격에 대한 변경이 있을 때

        [SerializeField] PlayerData _playerData;

        private void ChangeCharacterState(ECharacterStateType stateType)
        {
            if (_characterState == stateType) return;
            _characterState = stateType;

            if(stateType == ECharacterStateType.Idle || stateType == ECharacterStateType.Run)
            {
                _animatorController.SetInt(ConstString.PLAYER_STATE_KEY, (int)_characterState);
            }
        }
        public void SetGameModeChanger(IGameModeChanger modeChanger)
        {
            _gameModeChanger = modeChanger;
        }
        public override void Init()
        {
            base.Init();
            _levelUpXp = 1;//MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);

            _nextAttackBuff.Reset();

            MaxHp = new StatValue(_playerData.HP);
            MaxMoveCount = new StatValue(_playerData.MoveCount);
            Damage = new StatValue(_playerData.Damage);

            _currentHP = _playerData.HP;
            _currentMoveCount = _playerData.MoveCount;

            OnStatChanged?.Invoke(EPlayerStat.HP);
            OnStatChanged?.Invoke(EPlayerStat.MoveCount);
            OnStatChanged?.Invoke(EPlayerStat.Damage);

            _nextAttackBuff.Reset();
        }
        public void GetReward(RewardData rewardData)
        {
            _currentXp += rewardData.Xp;

            Debug.Log(_currentXp);

            while (_currentXp >= _levelUpXp)
            {
                int remainXp = _currentXp - _levelUpXp;
                _currentXp = remainXp;

                LevelUp();
            }
        }
        private void LevelUp()
        {
            Debug.Log("Level Up");
            _level++;
            _levelUpXp = 1;//MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.LevelUp);

            LevelUpEvent?.Invoke();
        }
        public void Move(EDirectionType inputDirection)
        {
            _gameModeChanger.EnterGameMode(EGameModeType.Sliding);

            if (_characterState != ECharacterStateType.Idle)
                return;

            Queue<MoveContext> moveQueue = _route.BuildRoute(TilePosition, inputDirection, GetMoveContextFunc);

            if(moveQueue.Count == 1)
            {
                SetCharacterRotation(inputDirection);
                FinishMove();
            }
            else
            {
                StartCoroutine(CoProcessMoveSequence(moveQueue));
            }
        }
        private IEnumerator CoProcessMoveSequence(Queue<MoveContext> moveContexts)
        {
            ChangeCharacterState(ECharacterStateType.Run);

            while (moveContexts.Count > 0)
            {
                MoveContext moveContext = moveContexts.Dequeue();

                if(moveContext.Damage != 0)
                    ApplyDamage(new DamageContext(null, moveContext.Damage, false));

                switch (moveContext.ResultType)
                {
                    case ESlideResultType.Move:
                        yield return StartCoroutine(CoMove(moveContext));
                        break;
                    case ESlideResultType.Stop:
                        break;
                    case ESlideResultType.EnemyStop:
                        break;
                    case ESlideResultType.Teleport:
                        SetPosition(moveContext.DestTile);
                        break;
                }
            }

            ChangeCharacterState(ECharacterStateType.Idle);
            FinishMove();
        }
        public void FinishMove()
        {
            FinishSlideEvent?.Invoke();
            ModifyStat(new PlayerApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, -1, EPlayerStat.None));
            _gameModeChanger.ExitGameMode(EGameModeType.Sliding);
        }
        private IEnumerator CoMove(MoveContext moveContext)
        {
            float lerpScale = 0;

            SetCharacterRotation(moveContext.Direction);
            Vector3 startPosition = this.transform.position;

            while (lerpScale < 1)
            {
                lerpScale += Time.deltaTime * ConstData.MOVE_LERP_SCALE;
                this.transform.position = Vector3.Lerp(startPosition, moveContext.DestTile.GetPosition, lerpScale);

                yield return null;
            }

            SetPosition(moveContext.DestTile);
        }
        public override void OnDeath()
        {
            base.OnDeath();
            OnDeathEvent?.Invoke();
        }
        public override void TakeDamage(DamageContext damageInfo)
        {
            base.TakeDamage(damageInfo);

            SetCharacterRotation(ToTargetDirection(damageInfo.Attacker.TilePosition));

            if(damageInfo.StatusEffect == EStatusEffectType.KnockBack)
            {
                MoveContext context = _route.GetBackRoute(TilePosition, damageInfo.Attacker.Direction, GetMoveContextFunc);
    
                if(context.DestTile != TilePosition)
                    StartCoroutine(CoKnockBack(context));
            }

            _animatorController.SetAnimationTrigger(ConstString.HIT_ANIM);
        }
        private IEnumerator CoKnockBack(MoveContext moveContext)
        {
            _isKnockbacking = true;

            float elapsed = 0f;
            float duration = 0.45f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                float eased = MathUtil.EaseOutCubic(t); // OutQuart/OutExpo로 바꿔도 됨

                this.transform.position = Vector3.LerpUnclamped(TilePosition.GetPosition, moveContext.DestTile.GetPosition, eased);

                yield return null;
            }

            _isKnockbacking = false;
            SetPosition(moveContext.DestTile);
        }
        public override void EndHittedAnimation()
        {
            if (_isKnockbacking == true)
                base.EndHittedAnimation();
            else
                StartCoroutine(CoWaitEndAction());
        }
        private IEnumerator CoWaitEndAction()
        {
            yield return new WaitUntil(()=> _isKnockbacking == false);
            base.EndHittedAnimation();
        }
        public override void Attack(ICombatant target)
        {
            base.Attack(target);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
        public void ModifyStat(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            switch (applyStatContext.PlayerStat)
            {
                case EPlayerStat.HP:

                    _currentHP = Mathf.Min(Get(EPlayerStat.HP) + intValue, Get(EPlayerStat.MaxHp));

                    break;
                case EPlayerStat.MaxHp:

                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            MaxHp.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            MaxHp.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            MaxHp.AddRatio(EPlayerStat.MaxHp, applyStatContext.Value);
                            break;
                    }

                    break;
                case EPlayerStat.Damage:

                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            Damage.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            Damage.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            Damage.AddRatio(EPlayerStat.MaxHp, applyStatContext.Value);
                            break;
                    }

                    break;
                case EPlayerStat.MoveCount:

                    _currentMoveCount = Mathf.Min(Get(EPlayerStat.MoveCount) + intValue, Get(EPlayerStat.MaxMoveCount));

                    break;
                case EPlayerStat.MaxMoveCount:
                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            MaxMoveCount.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            MaxMoveCount.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            MaxMoveCount.AddRatio(EPlayerStat.MaxHp, applyStatContext.Value);
                            break;
                    }
                    break;
            }
            OnStatChanged(applyStatContext.PlayerStat);
            Debug.Log("Stat Modifier");
        }
        private float CalculateModifyValue(PlayerApplyStatContext applyStatContext)
        {
            float value = 0;

            if(applyStatContext.ApplyType == EApplyStatType.Ratio)
            {
                value = Get(applyStatContext.RatioStatType) * applyStatContext.Value;
            }
            else
            {
                value = applyStatContext.Value;
            }

            return value;
        }
        public void GainBarrier()
        {
            throw new NotImplementedException();
        }
        public int SlideTileCount()
        {
            throw new NotImplementedException();
        }
        public bool IsRoute(EDirectionType dir)
        {
            throw new NotImplementedException();
        }
        public void MoveStep(EDirectionType dir, int stepCount = 1)
        {
            throw new NotImplementedException();
        }
        public override bool TrySubmitAttackRequest()
        {
            if (_sensor.GetCombatant(TilePosition.GetNextTile(Direction), ECretureType.Enemy, out var target))
            {
                AttackRequestListener.EnqueueActPair(new ActPair(this, target));
                return true;
            }
            else return false;
        }
        public bool TryGet<T>(out T service) where T : class
        {
            service = this as T;
            return service != null;
        }
        public int Get(EPlayerStat stat)
        {
            int value = 0;

            switch (stat)
            {
                case EPlayerStat.HP:
                    value = _currentHP;
                    break;
                case EPlayerStat.MaxHp:
                    value = MaxHp.Final(this);
                    break;
                case EPlayerStat.Damage:

                    int baseDamage = Damage.Final(this);
                    int multiDamage = (int)((baseDamage + _nextAttackBuff.NextExtraDamage)* _nextAttackBuff.NextExtraDamageMultiplier);
                    value = multiDamage;

                    Debug.Log(baseDamage);
                    Debug.Log(multiDamage);

                    break;
                case EPlayerStat.MoveCount:

                    value = _currentMoveCount;

                    break;
                case EPlayerStat.MaxMoveCount:
                    value = (int)(MaxMoveCount.Final(this));
                    break;
            }


            return value;
        }
        public void AddEnhance(ENextAttackEnhanceType nextAttackEnhanceType, float value)
        {
            switch (nextAttackEnhanceType)
            {
                case ENextAttackEnhanceType.Add:
                    _nextAttackBuff.AddDamage((int)value);
                    break;
                case ENextAttackEnhanceType.Multi:
                    _nextAttackBuff.AddDamageMulti(value);
                    break;
                case ENextAttackEnhanceType.ExtraAttack:
                    _nextAttackBuff.AddExtraAttack((int)value);
                    break;
            }

            OnStatChanged?.Invoke(EPlayerStat.Damage);
        }

        protected override DamageContext CreateDamageContext()
        {
            DamageContext damageInfo = new DamageContext(this, Get(EPlayerStat.Damage), false);
            return damageInfo;
        }

        public override void ReduceHP(int damage)
        {
            ModifyStat(new PlayerApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, -damage, EPlayerStat.None));
        }

        public override void TurnEnd()
        {
            base.TurnEnd();
            if(_currentHP == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByHP);

            if(_currentMoveCount == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByMoveCount);

        }

        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            throw new NotImplementedException();
        }
    }
}
