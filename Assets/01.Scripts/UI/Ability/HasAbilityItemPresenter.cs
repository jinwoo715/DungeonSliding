using JW.DungeonSliding.GamePlay.Ability;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class HasAbilityItemPresenter : MonoBehaviour
    {
        [SerializeField] private List<HasAbilityViewerItem> _abilityItems;

        private int _currentIndex = 0;

        ITooltipService _tooltipService;
        IAbilityEventService _abilityService;

        public void Init(ITooltipService tooltipService, IAbilityEventService abilityService)
        {
            _tooltipService = tooltipService;
            _abilityService = abilityService;

            _abilityService.OnAddedRuleAbility += AddAbility;
        }

        public void AddAbility(AbilityDataBase data)
        {
            if (_currentIndex >= _abilityItems.Count) return;

            HasAbilityViewerItem item = _abilityItems[_currentIndex++];
            item.gameObject.SetActive(true);
            item.SetData(data, _tooltipService.ShowTooltip, _tooltipService.CloseTooltip);
        }
    }
}
