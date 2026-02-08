using UnityEngine;
using System.Collections.Generic;
using System;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding
{
    public interface IRouteService
    {
        public Queue<MoveContext> BuildRoute(Tile startTile, EDirectionType direction, int maxCount);
        public int LastMoveTileCount { get; }
    }

    public class RouteBuilder : IRouteService
    {
        IMoveContextProvider _moveContextProvider;

        private Queue<MoveContext> _lastMoveRoute = new Queue<MoveContext>();
        public int LastMoveTileCount => _lastMoveRoute.Count-1;

        public RouteBuilder(IMoveContextProvider moveContextProvider)
        {
            _moveContextProvider = moveContextProvider;
        }

        public Queue<MoveContext> BuildRoute(Tile startPoint, EDirectionType moveDir, int maxCount)
        {
            _lastMoveRoute.Clear();

            Queue<MoveContext> moveQueue = new Queue<MoveContext>();

            ETileEnterType enterType = ETileEnterType.None;

            int moveCount = 0;
            while (moveCount < maxCount)
            {
                moveCount++;

                MoveContext moveContext = _moveContextProvider.GetMoveContext(startPoint, moveDir, enterType);
                moveQueue.Enqueue(moveContext);
                _lastMoveRoute.Enqueue(moveContext);
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
    }
}

