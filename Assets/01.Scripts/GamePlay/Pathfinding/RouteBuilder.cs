using UnityEngine;
using System.Collections.Generic;
using System;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding
{
    public interface IRouteService
    {
        public Queue<MoveContext> BuildRoute(Tile startTile, EDirectionType direction);
        public int LastMoveTileCount { get; }
    }

    public class RouteBuilder : IRouteService
    {
        IMoveContextProvider _moveContextProvider;

        private Queue<MoveContext> _lastMoveRoute = new Queue<MoveContext>();
        public int LastMoveTileCount => _lastMoveRoute.Count;

        public RouteBuilder(IMoveContextProvider moveContextProvider)
        {
            _moveContextProvider = moveContextProvider;
        }

        public Queue<MoveContext> BuildRoute(Tile startPoint, EDirectionType moveDir)
        {
            _lastMoveRoute.Clear();

            Queue<MoveContext> moveQueue = new Queue<MoveContext>();

            ETileEnterType enterType = ETileEnterType.None;

            while (true)
            {
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

