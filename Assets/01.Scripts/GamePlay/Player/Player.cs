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
    public class Player : Creature, IMoveable, IRewardReceiver
    {
        [SerializeField] private ObjectFader _objectFader;
        [SerializeField] private MoveController _moveController;
        [SerializeField] private BarrierVisualViewer _barrier;

        private ECharacterStateType _characterState = ECharacterStateType.Idle;
        public ESlideResultType SlideResultType { get; private set; }

        private IRouteService _routeService;
        private IMoveRule _moveRule;

        public event Action OnMoveEnd;
        public event Action OnSlideEnd;

        public event Action<int> OnGetXp;
        private void ChangeCharacterState(ECharacterStateType stateType)
        {
            if (_characterState == stateType) return;
            _characterState = stateType;

            if(stateType == ECharacterStateType.Idle || stateType == ECharacterStateType.Run)
            {
                _animatorController.SetInt(ConstString.PLAYER_STATE_KEY, (int)_characterState);
            }
        }
        public override void Initialize(ECreatureType cretureType)
        {
            base.Initialize(cretureType);
            _objectFader.Init();
        }
        public void Wire(IRouteService routeService, IMoveRule moveRule)
        {
            _routeService = routeService;
            _moveRule = moveRule;

            _moveController.Wire(_routeService, this);
            Bind();
        }
        private void Bind() 
        {
            _moveController.OnDirectionChanged += Rotate.SetRotation;
            _moveController.OnSlideStart += HandleSlideStart;
            _moveController.OnSlideEnd += HandleSlideEnd;
            _moveController.OnSlideBlocked += HandleSlideBlocked;
            _moveController.OnPushedEnd += HandleKnockBackEnd;
            _moveController.OnMoveEnd += () => OnMoveEnd?.Invoke();
            _moveController.OnStepOnEffectTile += () => Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnSteppedEffectTile);

            StatusModifier.OnAppliedStatus += (status) => 
            {
                if (status == ECreatureStatus.Barrier)
                    _barrier.ExcuteBarrier();

                if (status == ECreatureStatus.Hide)
                    _objectFader.FadeOut();
            };
            StatusModifier.OnReleasedStatus += (status) =>
            {
                if (status == ECreatureStatus.Barrier)
                    _barrier.BreakBarrier();

                if (status == ECreatureStatus.Hide)
                    _objectFader.FadeIn();
            };
        }
        public void AddReward(RewardData rewardData)
        {
            Debug.Log(rewardData.Xp);
            OnGetXp?.Invoke(rewardData.Xp);

            if (rewardData.RewardType == ERewardType.KillReward)
                Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnKilled);
        }
        public void HandleLevelUp(int level)
        {
            Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnLevelUp);
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
            ChangeCharacterState(ECharacterStateType.Run);
        }
        private void HandleSlideEnd()
        {
            StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, -_moveRule.MoveCost));
            ChangeCharacterState(ECharacterStateType.Idle);
            SlideResultType = ESlideResultType.None;
            Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnSlided);
        }
        private void HandleSlideBlocked(ESlideResultType slideResultType)
        {
            if (slideResultType == ESlideResultType.Stop)
                Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnBlockedByWall);
        }
        private void HandleKnockBackEnd()
        {
            base.EndHittedAnimation();
        }
        public int SlideTileCount()
        {
            return _routeService.LastMoveTileCount;
        }
        public void KnockBack(EDirectionType dir)
        {
            _moveController.KnockBack(dir);
        }
       
        #endregion

        #region Combat
        public override void TakeDamage(DamageContext damageInfo)
        {
            EDirectionType dir = DirectionUtility.GetDirFromTileToTile(Tile.TilePosition, damageInfo.Attacker.Tile.TilePosition);
            Rotate.SetRotation(dir);

            if(damageInfo.Status.ContainsKey(ECreatureStatus.Knockback))
            {
                EDirectionType toAttackDir = DirectionUtility.GetDirFromTileToTile(Tile.TilePosition, damageInfo.Attacker.Tile.TilePosition);
                EDirectionType backDirection = DirectionUtility.GetReverseDirection(toAttackDir);

                KnockBack(backDirection);
            }

            base.TakeDamage(damageInfo);
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
