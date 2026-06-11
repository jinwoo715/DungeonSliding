using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.UI;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class GameResultViewer : MonoBehaviour
    {
        private int _abilityIndex = 0;
        [SerializeField] private List<HasAbilityViewerItem> _items;
        [SerializeField] private GameTooltipViewer _abilityTooltipViewer;

        [SerializeField] private TMP_Text _infoText;

        private StringBuilder _resultText = new StringBuilder();

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
    }
}
