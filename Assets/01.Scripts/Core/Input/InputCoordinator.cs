
using JW.DungeonSliding.Map;
using System;
using UnityEngine;

namespace JW.DungeonSliding.Core.Inputs
{
    public class InputCoordinator : MonoBehaviour, IInputService
    {
        [SerializeField] PCInput _pcInput;
        [SerializeField] MobileInput _mobileInput;
        private IInputService _currentInput;

        public event Action<EDirectionType> OnMoveInput;

        public void Init()
        {
#if UNITY_STANDALONE

            Debug.Log("StandAlone");

            _pcInput.gameObject.SetActive(true);
            _mobileInput.gameObject.SetActive(false);
            _currentInput = _pcInput;
#else
            Debug.Log("Mobile");

            _pcInput.gameObject.SetActive(false);
            _mobileInput.gameObject.SetActive(true);
            _currentInput = _mobileInput;
#endif
            _currentInput.OnMoveInput += OnMoveInput;
        }
    }
}
