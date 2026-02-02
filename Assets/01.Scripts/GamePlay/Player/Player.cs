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
        public int Base;      // ±âº»°ª
        public int Add;       // µ¡¼À º¸³Ê½º(+)
        public float Mul;     // °ö¼À º¸³Ê½º(¡¿), ±âº» 1f

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

        public int Final(IPlayerStatProvider StatReadOnly) 
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
    public class Player : Creature, IMoveable, IAbilityHost, INextAttackEnhancer, IPlayerStatProvider, IPlayerStatModifier, IRewardReceiver
    {
        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType { get; private set; }
        public EDirectionType MoveDir { get; private set; }

        private RoutePlanner _route = new RoutePlanner();
        public Action<EPlayerStat> OnStatChanged { get; set; }

        public event Func<Tile, EDirectionType, ETileEnterType, MoveContext> GetMoveContextFunc;

        private IGameModeChanger _gameModeChanger;

        private bool _isKnockbacking = false;

        private StatValue MaxHp;
        private StatValue Damage;
        private StatValue MaxMoveCount;
        private int _currentMoveCount;
        private int _currentHP;

        private int _level = 1;
        private int _currentXp = 0;
        private int _levelUpXp = 0;

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
        public void SetGameModeChanger(IGameModeChanger modeChanger)
        {
            _gameModeChanger = modeChanger;
        }
        public override void Init(ICombatEventListener combatEventListener, ECretureType cretureType)
        {
            base.Init(combatEventListener, cretureType);
            _levelUpXp = 1;//MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);
            _nextAttackBuff.Reset();
        }
        public void SetData(PlayerData playerData, IGameModeChanger gameModeChanger)
        {
            IGameModeChanger _gameModeChanger = gameModeChanger;

            MaxHp = new StatValue(playerData.HP);
            MaxMoveCount = new StatValue(playerData.MoveCount);
            Damage = new StatValue(playerData.Damage);

            _currentHP = playerData.HP;
            _currentMoveCount = playerData.MoveCount;

            OnStatChanged?.Invoke(EPlayerStat.HP);
            OnStatChanged?.Invoke(EPlayerStat.MoveCount);
            OnStatChanged?.Invoke(EPlayerStat.Damage);
        }
        public void AddReward(RewardData rewardData)
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
            _level++;
            _levelUpXp = 1;//MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.LevelUp);
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
        public void FinishMove()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.SlideEnd);
            ModifyStat(new PlayerApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, -1, EPlayerStat.None));
            _gameModeChanger.ExitGameMode(EGameModeType.Sliding);
        }
        public override void OnDeath()
        {
            base.OnDeath();
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

                float eased = MathUtil.EaseOutCubic(t); // OutQuart/OutExpo·Î ¹Ù²ãµµ µÊ

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
        public override void StartAttackAnimation()
        {
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
            return true;
        }
        public void MoveStep(EDirectionType dir, int stepCount = 1)
        {
            
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
        protected override void ReduceHP(int damage)
        {
            ModifyStat(new PlayerApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, -damage, EPlayerStat.None));
        }
        public override void EndBattle()
        {
            base.EndBattle();
            if(_currentHP == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByHP);

            if(_currentMoveCount == 0)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByMoveCount);
        }
        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            return damageInfo;
        }
    }
}
