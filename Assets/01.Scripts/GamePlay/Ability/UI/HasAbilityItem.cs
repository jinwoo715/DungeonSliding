using JW.DungeonSliding.GamePlay.Ability;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JW.DungeonSliding.UI
{
    public class HasAbilityItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private AbilityDataBase _abilityData;

        private Action CloseTooltipEvent;
        private Action<TooltipRequest> ShowTooltipEvent;

        public void SetData(AbilityDataBase abilityData, Action<TooltipRequest> showEvent, Action closeEvent)
        {
            _abilityData = abilityData;
            CloseTooltipEvent = closeEvent;
            ShowTooltipEvent = showEvent;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipRequest request = new TooltipRequest();
            request.Name = _abilityData.Name;
            request.Description = AbilityTextFormatter.ConvertPlayerAbility(_abilityData);
            request.Anchor = TextAnchor.LowerLeft;

            ShowTooltipEvent?.Invoke(request);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CloseTooltipEvent?.Invoke();
        }
    }
}
