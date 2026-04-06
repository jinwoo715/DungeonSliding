using JW.DungeonSliding.Core.Inputs;
using System;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MobileInput : MonoBehaviour, IInputService
    {
        public event Action<EDirectionType> OnMoveInput;

        private void Update()
        {

        }
    }
}
