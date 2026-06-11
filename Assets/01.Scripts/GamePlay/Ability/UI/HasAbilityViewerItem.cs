using JW.DungeonSliding.GamePlay.Ability;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class HasAbilityViewerItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _abilityImage;

        private AbilityDataBase _abilityData;

        private Action CloseTooltipEvent;
        private Action<TooltipRequest> ShowTooltipEvent;

        public void SetData(AbilityDataBase abilityData, Action<TooltipRequest> showEvent, Action closeEvent)
        {
            _abilityData = abilityData;
            CloseTooltipEvent = closeEvent;
            ShowTooltipEvent = showEvent;
            _abilityImage.sprite = abilityData.AbilitySprite;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipRequest request = new TooltipRequest();
            request.Name = _abilityData.Name;
            request.Description = AbilityTextFormatter.ConvertPlayerAbility(_abilityData);
            request.Anchor = TextAnchor.LowerLeft;
            request.ItemPosition = this.transform.position;
            ShowTooltipEvent?.Invoke(request);

            Debug.Log("???");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CloseTooltipEvent?.Invoke();
        }
    }
}
