using UnityEngine;
using JW.DungeonSliding;
using System;

namespace JW.DungeonSliding.Core.Inputs
{
    public interface IInputService
    {
        public event Action<EDirectionType> OnMoveInput;
    }

    public class PCInput : MonoBehaviour, IInputService
    {
        public event Action<EDirectionType> OnMoveInput;

        private void Update()
        {
            EDirectionType dir = EDirectionType.None;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = EDirectionType.Left;
            if (Input.GetKeyDown(KeyCode.UpArrow)) dir = EDirectionType.Up;
            if (Input.GetKeyDown(KeyCode.RightArrow)) dir = EDirectionType.Right;
            if (Input.GetKeyDown(KeyCode.DownArrow)) dir = EDirectionType.Down;

            if (dir != EDirectionType.None)
                OnMoveInput?.Invoke(dir);
        }
    }


}
