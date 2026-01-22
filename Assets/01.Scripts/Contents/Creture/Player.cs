using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.SlidingPuzzle 
{
    public class Player : Creture, ICombatant
    {
        private RoutePlanner _route = new RoutePlanner();
        private ECharacterStateType _characterState = ECharacterStateType.Idle;

        public Func<TilePoint, EDirectionType, ETileEnterType, MoveContext> GetMoveContextFunc;
        public Action FinishSlideEvent;
        public event Action LevelUpEvent;

        private int _level = 1;
        private int _levelUpXp = 0;
        private int _currentXp = 0;
        private bool _isKnockbacking = false;
        private void ChangeCharacterState(ECharacterStateType stateType)
        {
            if (_characterState == stateType) return;
            _characterState = stateType;

            if(stateType == ECharacterStateType.Idle || stateType == ECharacterStateType.Run)
            {
                _animatorController.SetInt(ConstString.PLAYER_STATE_KEY, (int)_characterState);
            }
        }
        public override void Init()
        {
            base.Init();
            GameSceneManager.Instance.Reward.GetRewardEvent -= GetReward;
            GameSceneManager.Instance.Reward.GetRewardEvent += GetReward;
            _levelUpXp = MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);
        }
        private void GetReward(RewardData rewardData)
        {
            _currentXp += rewardData.Xp;

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
            _levelUpXp = MathUtil.GetFib(_level + ConstData.LEVELUP_XP_OFFSET);
            LevelUpEvent?.Invoke();
        }
        public void MoveRoute(EDirectionType inputDirection)
        {
            if (_characterState != ECharacterStateType.Idle)
                return;

            Queue<MoveContext> moveQueue = _route.BuildRoute(Point, inputDirection, GetMoveContextFunc);

            if(moveQueue.Count == 1)
            {
                SetCharacterRotation(inputDirection);
                FinishSlideEvent?.Invoke();
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

                ApplyDamage(new DamageInfo(null, moveContext.Damage, false));

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
            FinishSlideEvent?.Invoke();
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
            GameSceneManager.Instance.GameFail();
        }
        public override void GetHit(DamageInfo damageInfo)
        {
            base.GetHit(damageInfo);

            SetCharacterRotation(ToTargetDirection(damageInfo.Attacker.Point));

            if(damageInfo.StatusEffect == EStatusEffectType.KnockBack)
            {
                MoveContext context = _route.GetBackRoute(Point, damageInfo.Attacker.Direction, GetMoveContextFunc);
    
                if(context.DestTile != Point)
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

                this.transform.position = Vector3.LerpUnclamped(Point.GetPosition, moveContext.DestTile.GetPosition, eased);

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
        private void OnDestroy()
        {
            if (GameSceneManager.Instance != null)
                GameSceneManager.Instance.Reward.GetRewardEvent -= GetReward;
        }
    }
}
