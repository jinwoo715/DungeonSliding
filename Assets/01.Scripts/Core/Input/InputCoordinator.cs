
using JW.DungeonSliding.Map;
using System;
using UnityEngine;

namespace JW.DungeonSliding.Core.Inputs
{
    public class InputCoordinator
    {
        public event Func<bool> IsMoveableFlowFunc;
        private IMoveable _moveable;

        public void Init(IMoveable moveable)
        {
            _moveable = moveable;
        }

        public void OnInputHandle(EDirectionType directionType)
        {
            if(IsMoveableFlowFunc?.Invoke() == true)
            {
                _moveable.Move(directionType);
            }
        }
    }
}
