using System;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GameGuideCategoryViewer : MonoBehaviour
    {
        [SerializeField] private Toggle[] _categoryToggles;

        private Action<int> OnSwitchToggleEvent;

        public void Initialize(Action<int> toggleEvent)
        {
            OnSwitchToggleEvent = toggleEvent;

            for (int i = 0; i < _categoryToggles.Length; i++)
            {
                int index = i;

                _categoryToggles[i].onValueChanged.AddListener((value) => { SwitchToggle(value, index); });
            }
        }

        public void Init()
        {
            _categoryToggles[0].isOn = true;
        }

        private void SwitchToggle(bool value, int index)
        {
            if (value == false) return;

            OnSwitchToggleEvent?.Invoke(index);
        }
    }
}
