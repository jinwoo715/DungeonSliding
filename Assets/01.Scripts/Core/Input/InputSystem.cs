using UnityEngine;
using JW.DungeonSliding;
using System;

namespace JW.DungeonSliding.Core.Inputs
{
    public class InputSystem : MonoBehaviour
    {
        public Action<EDirectionType> OnInputEvnet;

        private void Update()
        {
            EDirectionType dir = EDirectionType.None;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) dir = EDirectionType.Left;
            if (Input.GetKeyDown(KeyCode.UpArrow)) dir = EDirectionType.Up;
            if (Input.GetKeyDown(KeyCode.RightArrow)) dir = EDirectionType.Right;
            if (Input.GetKeyDown(KeyCode.DownArrow)) dir = EDirectionType.Down;

            if (dir != EDirectionType.None)
                OnInputEvnet?.Invoke(dir);
        }
    }
}
