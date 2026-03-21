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

        public void Init(IMoveContextProvider moveContextProvider)
        {
            _moveContextProvider = moveContextProvider;
        }

        //계산을 먼저 다 하고 움직이는 이유
        //1. 계산을 먼저 다 하고 움직인다.
        //2. 움직이면사 현재 위치, 방향에 따라 바로 움직이며, Tile일 경우 Trigger로 작동한다.

        //TODO 왜 굳이 1번 방식으로 했는지 기억이 안남.
        //이유를 생각해 볼 필요가 있을듯.

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

