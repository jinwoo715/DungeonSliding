using JW.DungeonSliding.GamePlay.Context;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GamePopupViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _popupName;
        [SerializeField] private TMP_Text _popupInfo;
        [SerializeField] private TMP_Text _buttonText;
        [SerializeField] private Button _button;

        public event Action OnClickEvent;
        private Action _receiveEvent;
        public void Init()
        {
            _button.onClick.AddListener(OnClickButton);
            Debug.Log("Button Init");
        }

        public void SetData(string name, string info, ButtonSet buttonSet)
        {
            _popupName.text = name;
            _popupInfo.text = info;

            _buttonText.text = buttonSet.ButtonName;

            _receiveEvent = buttonSet.ButtonEvent;
        }

        public void OnClickButton()
        {
            Debug.Log("Press Button");
            OnClickEvent?.Invoke();
            _receiveEvent?.Invoke();
        }
    }


}
