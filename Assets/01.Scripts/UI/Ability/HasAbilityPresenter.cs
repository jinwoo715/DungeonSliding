using JW.DungeonSliding.GamePlay.Ability;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class HasAbilityPresenter : MonoBehaviour
    {
        [SerializeField] private HasAbilityItem _abilityItemPrefab;
        [SerializeField] private Transform _abilityParentTransform;

        ITooltipService _tooltipService;
        IAbilityEventService _abilityService;

        public void Initialize(ITooltipService tooltipService, IAbilityEventService abilityService)
        {
            _tooltipService = tooltipService;
            _abilityService = abilityService;

            _abilityService.OnAddedAbility += AddAbility;
        }

        public void AddAbility(AbilityDataBase data)
        {
            HasAbilityItem item = Instantiate(_abilityItemPrefab, _abilityParentTransform);
            item.SetData(data, _tooltipService.ShowTooltip, _tooltipService.CloseTooltip);
        }

        private void OnDisable()
        {
            _abilityService.OnAddedAbility -= AddAbility;
        }
    }
}
