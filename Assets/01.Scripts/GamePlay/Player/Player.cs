using DG.Tweening;
using JW.DungeonSliding.Core;
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
        public int Base;      // ±âº»°ª
        public int Add;       // µ¡¼À º¸³Ê½º(+)
        public float Mul;     // °ö¼À º¸³Ê½º(¡¿), ±âº» 1f

        public Dictionary<EPlayerStatType, float> RatioValueByStat;

        public StatValue(int baseValue)
        {
            Base = baseValue;
            Add = 0;
            Mul = 1;

            RatioValueByStat = new();
        }

        public void AddRatio(EPlayerStatType stat, float value)
        {
            if (!RatioValueByStat.ContainsKey(stat))
                RatioValueByStat.Add(stat, 0);

            RatioValueByStat[stat] += value;
        }

        public int Final(IPlayerStatReader StatReadOnly) 
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
    public class Player : Creature, IMoveable, IAbilityHost, INextAttackEnhancer, IPlayerStatReader, IPlayerStatModifier, IRewardReceiver
    {
        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType { get; private set; }

        public event Action<EPlayerStatType> OnStatChanged;

        private ITileCheckService _tileCheckService;
        private IRouteService _routeService;
        private IMoveRule _moveRule;

        private bool _isPushed = false;

        private StatValue MaxHp;
        private StatValue Damage;
        private StatValue MaxMoveCount;
        private int _currentMoveCount;
        private int _currentHP;

        [SerializeField] private int _level = 1;
        [SerializeField] private int _currentXp = 0;
        [SerializeField] private int _requireXp = 0;

        [SerializeField] private GameObject _barrierObj;

        private NextAttackBuff _nextAttackBuff;
        
        private void ChangeCharacterState(ECharacterStateType stateType)
        {
            if (_characterState == stateType) return;
            _characterState = stateType;

            if(stateType == ECharacterStateType.Idle || stateType == ECharacterStateType.Run)
            {
                _animatorController.SetInt(ConstString.PLAYER_STATE_KEY, (int)_characterState);
            }
        }
        public override void Init(ICombatEventListener combatEventListener, ECretureType cretureType)
        {
            base.Init(combatEventListener, cretureType);
            _requireXp = MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);
            _nextAttackBuff.Reset();
        }
        public void SetData(PlayerData player, IRouteService routeService, ITileCheckService tileCheckService, IMoveRule moveRule)
        {
            MaxHp = new StatValue(player.HP);
            MaxMoveCount = new StatValue(player.MoveCount);
            Damage = new StatValue(player.Damage);

            _currentHP = player.HP;
            _currentMoveCount = player.MoveCount;

            OnStatChanged?.Invoke(EPlayerStatType.CurrentHP);
            OnStatChanged?.Invoke(EPlayerStatType.CurrentMoveCount);
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
            OnStatChanged?.Invoke(EPlayerStatType.Level);
            OnStatChanged?.Invoke(EPlayerStatType.CurrentXp);

            _tileCheckService = tileCheckService;
            _routeService = routeService;
            _moveRule = moveRule;
        }
        public void AddReward(RewardData rewardData)
        {
            Debug.Log("Reward");

            _currentXp += rewardData.Xp;

            while (_currentXp >= _requireXp)
            {
                int remainXp = _currentXp - _requireXp;
                _currentXp = remainXp;

                LevelUp();
            }

            OnStatChanged?.Invoke(EPlayerStatType.CurrentXp);
        }
        private void LevelUp()
        {
            _level++;
            _requireXp = MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);

            OnStatChanged?.Invoke(EPlayerStatType.Level);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnLevelUp);
        }

        #region Move
        public void SlideRoute(EDirectionType inputDirection)
        {
            if (!_moveRule.IsCanMove(inputDirection)) return;

            if (_isPushed) return;

            if (_characterState != ECharacterStateType.Idle) return;

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideStart);

            Queue<MoveContext> moveQueue = _routeService.BuildRoute(TilePosition, inputDirection, 100);

            if(moveQueue.Count == 1)
            {
                SetRotation(inputDirection);

                MoveContext cur = moveQueue.Dequeue();
                SlideResultType = cur.ResultType;
                if (cur.ResultType == ESlideResultType.Stop)
                    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideBlocked);

                FinishMove();
            }
            else
            {
                StartCoroutine(CoProcessMoveSequence(moveQueue));
            }
        }
        private IEnumerator CoProcessMoveSequence(Queue<MoveContext> moveContexts, bool lookDir = true)
        {
            ChangeCharacterState(ECharacterStateType.Run);

            while (moveContexts.Count > 0)
            {
                MoveContext moveContext = moveContexts.Dequeue();
                SlideResultType = moveContext.ResultType;

                switch (moveContext.ResultType)
                {
                    case ESlideResultType.Move:
                        if(lookDir) SetRotation(moveContext.Direction);
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

                if (moveContext.OnEnterEffectTile)
                {
                    moveContext.OnStepEvent?.Invoke();
                    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnStepEffectTile);
                }
            }

            ChangeCharacterState(ECharacterStateType.Idle);
            ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentMoveCount, EApplyStatType.Add, EPlayerStatType.None, -_moveRule.MoveCost));
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideEnd);
            FinishMove();
        }
        private IEnumerator CoMove(MoveContext moveContext)
        {
            float lerpScale = 0;
            
            Vector3 startPosition = this.transform.position;
            Vector3 endPosition = moveContext.DestTile.GetPosition;
            while (lerpScale < 1)
            {
                lerpScale += Time.deltaTime * ConstData.MOVE_LERP_SCALE;
                this.transform.position = Vector3.Lerp(startPosition, endPosition, lerpScale);

                yield return null;
            }

            SetPosition(moveContext.DestTile);
        }

        public void FinishMove()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnMoveEnd);
            SlideResultType = ESlideResultType.None;
        }
        public int SlideTileCount()
        {
            return _routeService.LastMoveTileCount;
        }

        public void KnockBack(EDirectionType dir)
        {
            StartCoroutine(CoKnockBack(dir));
        }

        public IEnumerator CoKnockBack(EDirectionType dir)
        {
            Queue<MoveContext> moveQueue = _routeService.BuildRoute(TilePosition, dir, 2);

            if (moveQueue.Count > 1)
            {
                MoveContext first = moveQueue.Dequeue();
                MoveContext second = moveQueue.Dequeue();

                yield return StartCoroutine(CoPushed(first.DestTile));

                if(second.ResultType == ESlideResultType.Teleport)
                    SetPosition(second.DestTile);

                FinishMove();
            }
        }
        private IEnumerator CoPushed(Tile backTile)
        {
            _isPushed = true;

            float elapsed = 0f;
            float duration = 0.45f;

            Vector3 startPosition = TilePosition.GetPosition;
            Vector3 endPosition = backTile.GetPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                float eased = MathUtil.EaseOutCubic(t); // OutQuart/OutExpo·Î ¹Ù²ãµµ µÊ

                this.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, eased);

                yield return null;
            }

            _isPushed = false;

            SetPosition(backTile);
        }
        #endregion

        #region Combat
        public override void OnDeath()
        {
            base.OnDeath();
            _isPushed = false;
        }
        public override bool TakeDamage(DamageContext damageInfo)
        {
            if (base.TakeDamage(damageInfo))
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDamaged);
            }

            SetRotation(DirectionToTile(damageInfo.Attacker.TilePosition));

            if(damageInfo.StatusEffect == EStatusEffectType.KnockBack)
            {
                EDirectionType toAttackDir = DirectionToTile(damageInfo.Attacker.TilePosition);
                EDirectionType backDirection = ReverseDirection(toAttackDir);
                Tile backTile = TilePosition.GetNextTile(backDirection);

                if (_tileCheckService.IsRouteTile(backTile))
                {
                    StartCoroutine(CoPushed(backTile));
                }
            }

            _animatorController.SetAnimationTrigger(ConstString.HIT_ANIM);

            return true;
        }
        public override void EndHittedAnimation()
        {
            if (_isPushed == false)
                base.EndHittedAnimation();
            else
                StartCoroutine(CoWaitEndAction());
        }
        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }

        protected override void ApplyAttack()
        {
            for (int i = 0; i < 1 + _nextAttackBuff.NextExtraAttackAcount; i++)
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnAttack);
                base.ApplyAttack();
            }

            ClearEnhance();
        }
        protected override void ReduceHP(int damage)
        {
            ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentHP, EApplyStatType.Add, EPlayerStatType.None, -damage));
        }
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();

            if (_currentHP == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByHP);

            if(_currentMoveCount == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByMoveCount);

            if(_currentHP <= 0 || _currentMoveCount <= 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeath);

            if (_currentHP <= 0 || _currentMoveCount <= 0)
                OnDeath();
        }


        private IEnumerator CoWaitEndAction()
        {
            yield return new WaitUntil(()=> _isPushed == false);
            base.EndHittedAnimation();
        }
        protected override DamageContext CreateDamageContext()
        {
            DamageContext damageInfo = new DamageContext(this, Get(EPlayerStatType.Damage), false);
            return damageInfo;
        }
        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            damageInfo.Damage = (int)(damageInfo.Damage * DamageTakenMultiplier);
            return damageInfo;
        }
        #endregion

        #region Stat
        public void ModifyStat(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            switch (applyStatContext.PlayerStat)
            {
                case EPlayerStatType.CurrentHP:

                    _currentHP = Mathf.Min(Get(EPlayerStatType.CurrentHP) + intValue, Get(EPlayerStatType.MaxHp));

                    break;
                case EPlayerStatType.MaxHp:

                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            MaxHp.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            MaxHp.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            MaxHp.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
                            break;
                    }

                    break;
                case EPlayerStatType.Damage:

                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            Damage.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            Damage.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            Damage.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
                            break;
                    }

                    break;
                case EPlayerStatType.CurrentMoveCount:

                    _currentMoveCount = Mathf.Min(Get(EPlayerStatType.CurrentMoveCount) + intValue, Get(EPlayerStatType.MaxMoveCount));
                    break;
                case EPlayerStatType.MaxMoveCount:
                    switch (applyStatContext.ApplyType)
                    {
                        case EApplyStatType.Add:
                            MaxMoveCount.Add += (int)applyStatContext.Value;
                            break;
                        case EApplyStatType.Multiple:
                            MaxMoveCount.Mul += applyStatContext.Value;
                            break;
                        case EApplyStatType.Ratio:
                            MaxMoveCount.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
                            break;
                    }
                    break;
            }
            OnStatChanged?.Invoke(applyStatContext.PlayerStat);
        }
        public void SetCurrentHP(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            _currentHP = Mathf.Min(intValue, Get(EPlayerStatType.MaxHp));

            OnStatChanged?.Invoke(applyStatContext.PlayerStat);
        }
        public void SetCurrentMoveCount(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            _currentMoveCount = Mathf.Min(intValue, Get(EPlayerStatType.MaxMoveCount));

            OnStatChanged?.Invoke(applyStatContext.PlayerStat);
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
        public int Get(EPlayerStatType stat)
        {
            int value = 0;

            switch (stat)
            {
                case EPlayerStatType.CurrentHP:
                    value = _currentHP;
                    break;
                case EPlayerStatType.MaxHp:
                    value = MaxHp.Final(this);
                    break;
                case EPlayerStatType.Damage:

                    int baseDamage = Damage.Final(this);
                    int multiDamage = (int)((baseDamage + _nextAttackBuff.NextExtraDamage)* _nextAttackBuff.NextExtraDamageMultiplier);
                    int finalDamage = (int)(multiDamage * DamageDealtMultiplier);
                    value = finalDamage;

                    break;
                case EPlayerStatType.CurrentMoveCount:

                    value = _currentMoveCount;

                    break;
                case EPlayerStatType.MaxMoveCount:
                    value = (int)(MaxMoveCount.Final(this));
                    break;

                case EPlayerStatType.Level:
                    value = _level;
                    break;

                case EPlayerStatType.CurrentXp:
                    value = _currentXp;
                    break;
                case EPlayerStatType.RequiredXp:
                    value = _requireXp;
                    break;
            }


            return value;
        }
        public void AddEnhance(ENextAttackType nextAttackEnhanceType, float value)
        {
            switch (nextAttackEnhanceType)
            {
                case ENextAttackType.Add:
                    _nextAttackBuff.AddDamage((int)value);
                    break;
                case ENextAttackType.Multiple:
                    _nextAttackBuff.AddDamageMulti(value);
                    break;
                case ENextAttackType.ExtraAttack:
                    _nextAttackBuff.AddExtraAttack((int)value);
                    break;
            }

            OnStatChanged?.Invoke(EPlayerStatType.Damage);
        }

        public override void AddDamageDealtMultiplier(float value)
        {
            base.AddDamageDealtMultiplier(value);
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
        }
        public void ClearEnhance()
        {
            _nextAttackBuff.Reset();
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
        }
        #endregion

        public override void GainBarrier()
        {
            base.GainBarrier();
            _barrierObj.SetActive(IsBarrierActive);
        }
        public override void ReleaseBarrier()
        {
            base.ReleaseBarrier();
            _barrierObj.SetActive(IsBarrierActive);
        }

        private void OnDisable()
        {
            _isPushed = false;
        }
    }
}
