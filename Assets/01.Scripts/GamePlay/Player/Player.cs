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
    public class Player : Creature, IMoveable, IAbilityHost, IRewardReceiver
    {
        [SerializeField] private MoveController _moveController;
        private LevelSystem _leveling = new LevelSystem();
        
        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType { get; private set; }

        private ITileCheckService _tileCheckService;
        private IRouteService _routeService;
        private IMoveRule _moveRule;

        private void ChangeCharacterState(ECharacterStateType stateType)
        {
            if (_characterState == stateType) return;
            _characterState = stateType;

            if(stateType == ECharacterStateType.Idle || stateType == ECharacterStateType.Run)
            {
                _animatorController.SetInt(ConstString.PLAYER_STATE_KEY, (int)_characterState);
            }
        }
        public override void Initialize(ECreatureType cretureType, IAttackRequestListener attackRequestListener)
        {
            base.Initialize(cretureType, attackRequestListener);
            _leveling.Initialize(1, 0);
        }
        public void SetData(IRouteService routeService, ITileCheckService tileCheckService, IMoveRule moveRule)
        {
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
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnLevelUp);
        }
        private void HandleXpChanged(int curXp, int requXp)
        {
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
            StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, StatModifyType.Add, -_moveRule.MoveCost));
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
        #endregion

        public void SetMoveResult(ESlideResultType result)
        {
            SlideResultType = result;
        }
    }
}
