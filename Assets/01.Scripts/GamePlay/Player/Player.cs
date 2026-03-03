using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using JW.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Player : Creature, IMoveable, IAbilityHost, INextAttackEnhancer, IStatReadOnly, IPlayerStatModifier, IRewardReceiver
    {
        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType { get; private set; }

        public event Action<EPlayerStatType> OnStatChanged;

        private ITileCheckService _tileCheckService;
        private IRouteService _routeService;
        private IMoveRule _moveRule;

        LevelSystem _leveling = new LevelSystem();
        [SerializeField] private MoveController _moveController;


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
        public override void Init(ICombatEventListener combatEventListener, ECreatureType cretureType)
        {
            base.Init(combatEventListener, cretureType);
            _leveling.Initialize(1, 0);
            _nextAttackBuff.Reset();
        }
        public void SetData(PlayerData player, IRouteService routeService, ITileCheckService tileCheckService, IMoveRule moveRule)
        {
            //MaxHp = new StatValue(player.HP);
            //MaxMoveCount = new StatValue(player.MoveCount);
            //Damage = new StatValue(player.Damage);

            //_currentHP = player.HP;
            //_currentMoveCount = player.MoveCount;

            OnStatChanged?.Invoke(EPlayerStatType.CurrentHP);
            OnStatChanged?.Invoke(EPlayerStatType.CurrentMoveCount);
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
                 //수정
                 //OnStatChanged?.Invoke(EPlayerStatType.Level);
                 //OnStatChanged?.Invoke(EPlayerStatType.CurrentXp);

            _tileCheckService = tileCheckService;
            _routeService = routeService;
            _moveRule = moveRule;

            _leveling.OnLevelUp += HandleLevelUp;
            _leveling.OnChangedXp += HandleXpChanged;

            _moveController.Initialize(_routeService, this);
            _moveController.OnDirectionChanged += Rotate.SetRotation;
            _moveController.OnSlideStart += HandleSlideStart;
            _moveController.OnSlideEnd += HandleSlideEnd;
            _moveController.OnSlideBlocked += HandleSlideBlocked;
            _moveController.OnPushedEnd += HandleKnockBackEnd;
        }
        public void AddReward(RewardData rewardData)
        {
            _leveling.AddXp(rewardData.Xp);
        }
        private void HandleLevelUp(int level)
        {
            OnStatChanged?.Invoke(EPlayerStatType.Level);
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnLevelUp);
        }
        private void HandleXpChanged(int curXp, int requXp)
        {
            OnStatChanged?.Invoke(EPlayerStatType.CurrentXp);
        }

        #region Move
        public void SlideRoute(EDirectionType inputDirection)
        {
            if (!_moveRule.IsCanMove(inputDirection)) return;

            if (_moveController.IsMoving) return;

            if (_characterState != ECharacterStateType.Idle) return;

            _moveController.SlideRoute(inputDirection);
        }

        private void HandleSlideStart()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideStart);
            ChangeCharacterState(ECharacterStateType.Run);
        }
        private void HandleSlideEnd()
        {
            ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentMoveCount, EApplyStatType.Add, EPlayerStatType.None, -_moveRule.MoveCost));
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideEnd);
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnMoveEnd);
            ChangeCharacterState(ECharacterStateType.Idle);
            SlideResultType = ESlideResultType.None;
        }
        private void HandleSlideBlocked()
        {
            if (SlideResultType == ESlideResultType.Stop)
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnSlideBlocked);
        }
        private void HandleKnockBackEnd()
        {
            base.EndHittedAnimation();
        }

        public int SlideTileCount()
        {
            return _routeService.LastMoveTileCount;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                KnockBack(GridUtility.GetReverseDirection(Rotate.Direction));
            }
        }

        public void KnockBack(EDirectionType dir)
        {
            _moveController.KnockBack(dir);
        }
       
        #endregion

        #region Combat
        public override void OnDeath()
        {
            base.OnDeath();
        }
        public override void TakeDamage(DamageContext damageInfo)
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDamaged);

            EDirectionType dir = GridUtility.GetDirFromTileToTile(Tile.TilePosition, damageInfo.Attacker.Tile.TilePosition);
            Rotate.SetRotation(dir);

            if(damageInfo.Status.ContainsKey(EStatusEffectType.KnockBack))
            {
                EDirectionType toAttackDir = GridUtility.GetDirFromTileToTile(Tile.TilePosition, damageInfo.Attacker.Tile.TilePosition);
                EDirectionType backDirection = GridUtility.GetReverseDirection(toAttackDir);
                Tile backTile = Tile.TilePosition.GetNextTileByDir(backDirection);

                if (_tileCheckService.IsRouteTile(backTile))
                {
                    KnockBack(backDirection);
                }
            }

            _animatorController.SetAnimationTrigger(ConstString.HIT_ANIM);
        }
        public override void EndHittedAnimation()
        {
            if (!_moveController.IsMoving)
                base.EndHittedAnimation();
        }
        public void ExcuteAttack()
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
        protected void ReduceHP(int damage)
        {
            ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentHP, EApplyStatType.Add, EPlayerStatType.None, -damage));
        }
        public override void OnBattleEnd()
        {
            base.OnBattleEnd();

            //if (_currentHP == 0)
            //    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByHP);

            //if(_currentMoveCount == 0)
            //    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeathByMoveCount);

            //if(_currentHP <= 0 || _currentMoveCount <= 0)
            //    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnDeath);

            //if (_currentHP <= 0 || _currentMoveCount <= 0)
            //    OnDeath();
        }

        #endregion

        #region Stat
        public void ModifyStat(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            //switch (applyStatContext.PlayerStat)
            //{
            //    case EPlayerStatType.CurrentHP:

            //        _currentHP = Mathf.Min(Get(EPlayerStatType.CurrentHP) + intValue, Get(EPlayerStatType.MaxHp));

            //        break;
            //    case EPlayerStatType.MaxHp:

            //        switch (applyStatContext.ApplyType)
            //        {
            //            case EApplyStatType.Add:
            //                MaxHp.Add += (int)applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Multiple:
            //                MaxHp.Mul += applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Ratio:
            //                MaxHp.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
            //                break;
            //        }

            //        break;
            //    case EPlayerStatType.Damage:

            //        switch (applyStatContext.ApplyType)
            //        {
            //            case EApplyStatType.Add:
            //                Damage.Add += (int)applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Multiple:
            //                Damage.Mul += applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Ratio:
            //                Damage.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
            //                break;
            //        }

            //        break;
            //    case EPlayerStatType.CurrentMoveCount:

            //        _currentMoveCount = Mathf.Min(Get(EPlayerStatType.CurrentMoveCount) + intValue, Get(EPlayerStatType.MaxMoveCount));
            //        break;
            //    case EPlayerStatType.MaxMoveCount:
            //        switch (applyStatContext.ApplyType)
            //        {
            //            case EApplyStatType.Add:
            //                MaxMoveCount.Add += (int)applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Multiple:
            //                MaxMoveCount.Mul += applyStatContext.Value;
            //                break;
            //            case EApplyStatType.Ratio:
            //                MaxMoveCount.AddRatio(applyStatContext.RatioStatType, applyStatContext.Value);
            //                break;
            //        }
            //        break;
            //}
            OnStatChanged?.Invoke(applyStatContext.PlayerStat);
        }
        public void SetCurrentHP(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

            //_currentHP = Mathf.Min(intValue, Get(EPlayerStatType.MaxHp));

            OnStatChanged?.Invoke(applyStatContext.PlayerStat);
        }
        public void SetCurrentMoveCount(PlayerApplyStatContext applyStatContext)
        {
            float floatValue = CalculateModifyValue(applyStatContext);
            int intValue = Mathf.RoundToInt(floatValue);

           // _currentMoveCount = Mathf.Min(intValue, Get(EPlayerStatType.MaxMoveCount));

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

            //switch (stat)
            //{
            //    case EPlayerStatType.CurrentHP:
            //        value = _currentHP;
            //        break;
            //    case EPlayerStatType.MaxHp:
            //        value = MaxHp.Final(this);
            //        break;
            //    case EPlayerStatType.Damage:

            //        int baseDamage = Damage.Final(this);
            //        int multiDamage = (int)((baseDamage + _nextAttackBuff.NextExtraDamage)* _nextAttackBuff.NextExtraDamageMultiplier);
            //        int finalDamage = (int)(multiDamage * DamageDealtMultiplier);
            //        value = finalDamage;

            //        break;
            //    case EPlayerStatType.CurrentMoveCount:

            //        value = _currentMoveCount;

            //        break;
            //    case EPlayerStatType.MaxMoveCount:
            //        value = (int)(MaxMoveCount.Final(this));
            //        break;

                    //TODO 수정
                //case EPlayerStatType.Level:
                //    value = _level;
                //    break;

                //case EPlayerStatType.CurrentXp:
                //    value = _currentXp;
                //    break;
                //case EPlayerStatType.RequiredXp:
                //    value = _requireXp;
                //    break;
            //}


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

        public void AddDamageDealtMultiplier(float value)
        {
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
        }
        public void ClearEnhance()
        {
            _nextAttackBuff.Reset();
            OnStatChanged?.Invoke(EPlayerStatType.Damage);
        }
        #endregion


        public void SetMoveResult(ESlideResultType result)
        {
            SlideResultType = result;
        }
        public int Get(ECreatureStatType stat)
        {
            throw new NotImplementedException();
        }
    }
}
