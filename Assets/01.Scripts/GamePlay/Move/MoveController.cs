using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.Map;
using JW.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MoveController : MonoBehaviour
    {
        private IRouteService _routeService;
        private IMoveable _moveable;

        public event Action OnSlideStart;
        public event Action OnSlideEnd;
        public event Action<ESlideResultType> OnSlideBlocked;
        public event Action OnMoveEnd;
        public event Action<EDirectionType> OnDirectionChanged;
        public event Action OnStepOnEffectTile;

        public event Action OnPushedStart;
        public event Action OnPushedEnd;
        public bool IsMoving { get; private set; }

        public void Wire(IRouteService routeService, IMoveable moveable)
        {
            _routeService = routeService;
            _moveable = moveable;
        }

        public void SlideRoute(EDirectionType inputDirection)
        {
            IsMoving = true;

            Queue<MoveContext> moveQueue = _routeService.BuildRoute(_moveable.Tile.TilePosition, inputDirection, 100);

            if (moveQueue.Count == 1)
            {
                OnDirectionChanged?.Invoke(inputDirection);

                MoveContext cur = moveQueue.Dequeue();
                _moveable.SetMoveResult(cur.ResultType);

                OnSlideBlocked?.Invoke(cur.ResultType);

                FinishMove();
            }
            else
            {
                OnSlideStart?.Invoke();
                StartCoroutine(CoProcessMoveSequence(moveQueue));
            }
        }
        private IEnumerator CoProcessMoveSequence(Queue<MoveContext> moveContexts)
        {
            while (moveContexts.Count > 0)
            {
                MoveContext moveContext = moveContexts.Dequeue();
                _moveable.SetMoveResult(moveContext.ResultType);

                OnDirectionChanged?.Invoke(moveContext.Direction);

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
                        _moveable.Tile.SetPosition(moveContext.DestTile);
                        break;
                }

                if (moveContext.OnEnterEffectTile)
                {
                    moveContext.OnStepEvent?.Invoke();
                    OnStepOnEffectTile?.Invoke();
                }
            }
            OnSlideEnd?.Invoke();
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

            _moveable.Tile.SetPosition(moveContext.DestTile);
        }
        private void FinishMove()
        {
            OnMoveEnd?.Invoke();
            IsMoving = false;
        }
        public void KnockBack(EDirectionType dir)
        {
            StartCoroutine(CoKnockBack(dir));
        }
        public IEnumerator CoKnockBack(EDirectionType dir)
        {
            IsMoving = true;

            Queue<MoveContext> moveQueue = _routeService.BuildRoute(_moveable.Tile.TilePosition, dir, 2);

            if (moveQueue.Count > 1)
            {
                OnPushedStart?.Invoke();

                MoveContext first = moveQueue.Dequeue();
                MoveContext second = moveQueue.Dequeue();

                yield return StartCoroutine(CoPushed(first.DestTile));

                if (second.ResultType == ESlideResultType.Teleport)
                    _moveable.Tile.SetPosition(second.DestTile);
            }
            IsMoving = false;
        }
        private IEnumerator CoPushed(Tile backTile)
        {
            float elapsed = 0f;
            float duration = 0.45f;

            Vector3 startPosition = _moveable.Tile.TilePosition.GetPosition;
            Vector3 endPosition = backTile.GetPosition;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                float eased = MathUtil.EaseOutCubic(t); // OutQuart/OutExpo·Î ¹Ù²ãµµ µÊ

                this.transform.position = Vector3.LerpUnclamped(startPosition, endPosition, eased);

                yield return null;
            }

            _moveable.Tile.SetPosition(backTile);

            IsMoving = false;
            OnPushedEnd?.Invoke();
        }
    }
}
