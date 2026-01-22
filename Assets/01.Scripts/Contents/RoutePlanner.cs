using UnityEngine;
using System.Collections.Generic;
using System;

namespace JW.SlidingPuzzle
{
    public class RoutePlanner
    {
        public Queue<MoveContext> BuildRoute(
            TilePoint startPoint,
            EDirectionType moveDir,
            Func<TilePoint, EDirectionType, ETileEnterType, MoveContext> getMoveContext)
        {
            if (getMoveContext == null) return null;

            Queue<MoveContext> moveQueue = new Queue<MoveContext>();

            ETileEnterType enterType = ETileEnterType.None;

            while (true)
            {
                MoveContext moveContext = getMoveContext(startPoint, moveDir, enterType);
                moveQueue.Enqueue(moveContext);

                ESlideResultType slideResultType = moveContext.ResultType;

                if (slideResultType == ESlideResultType.Stop || slideResultType == ESlideResultType.EnemyStop)
                {
                    break;
                }

                startPoint = moveContext.DestTile;
                moveDir = moveContext.Direction;
                enterType = moveContext.EnterType;
            }

            return moveQueue;
        }

        public MoveContext GetBackRoute(
            TilePoint startPoint,
            EDirectionType moveDir,
            Func<TilePoint, EDirectionType, ETileEnterType, MoveContext> getMoveContext)
        {
            MoveContext moveContext = getMoveContext.Invoke(startPoint, moveDir, ETileEnterType.Slide);

            return moveContext;
        }
    }
}

