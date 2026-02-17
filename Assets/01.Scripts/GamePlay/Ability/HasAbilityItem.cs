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
            Debug.Log("On Pointer Down");
            TooltipRequest request = new TooltipRequest();
            request.Name = _abilityData.Name;
            request.Description = _abilityData.Description;
            request.Position = eventData.position;

            ShowTooltipEvent?.Invoke(request);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("On Pointer Up");
            CloseTooltipEvent?.Invoke();
        }
    }
}
