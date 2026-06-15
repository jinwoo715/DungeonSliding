using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.UI;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GameResultViewer : MonoBehaviour
    {
        private int _abilityIndex = 0;
        [SerializeField] private List<HasAbilityViewerItem> _items;
        [SerializeField] private GameTooltipViewer _abilityTooltipViewer;

        [SerializeField] private TMP_Text _infoText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _toLobbyButton;

        private StringBuilder _resultText = new StringBuilder();
        private Action _retryAction;
        private Action _toLobbyAction;

        public void Init(Action retryAction, Action toLobbyAction)
        {
            _retryAction = retryAction;
            _toLobbyAction = toLobbyAction;

            _retryButton.onClick.RemoveListener(OnClickRetry);
            _toLobbyButton.onClick.RemoveListener(OnClickToLobby);

            _retryButton.onClick.AddListener(OnClickRetry);
            _toLobbyButton.onClick.AddListener(OnClickToLobby);
        }

        public void AppendInfo(string text)
        {
            _resultText.Append(text);
            AppendEnter();
        }
        public void AppendEnter()
        {
            _resultText.Append("\n");
        }
        public void PrintResult()
        {
            _infoText.text = _resultText.ToString();
        }

        public void AbilityClear()
        {
            foreach (var ability in _items)
            {
                ability.gameObject.SetActive(false);
            }
        }

        public void SetAbilityData(AbilityDataBase data)
        {


            HasAbilityViewerItem item = _items[_abilityIndex++];
            item.gameObject.SetActive(true);
            item.SetData(data, ShowAbilityPopup, CloseAblityPopup);
        }
        private void ShowAbilityPopup(TooltipRequest tooltipRequest)
        {
            _abilityTooltipViewer.gameObject.SetActive(true);
            _abilityTooltipViewer.SetData(tooltipRequest.IconSprite, tooltipRequest.Name, tooltipRequest.Description);
            _abilityTooltipViewer.transform.position = tooltipRequest.ItemPosition;
        }
        private void CloseAblityPopup()
        {
            _abilityTooltipViewer.gameObject.SetActive(false);
        }

        private void OnClickRetry()
        {
            GameManager.Sound.PlayEffectSound(EEffectSoundType.PressButton);
            _retryAction?.Invoke();
        }

        private void OnClickToLobby()
        {
            GameManager.Sound.PlayEffectSound(EEffectSoundType.PressButton);
            _toLobbyAction?.Invoke();
        }

        private void OnDestroy()
        {
            if (_retryButton != null)
                _retryButton.onClick.RemoveListener(OnClickRetry);

            if (_toLobbyButton != null)
                _toLobbyButton.onClick.RemoveListener(OnClickToLobby);
        }
    }
}
