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

        public void Init(ITooltipService tooltipService, IAbilityEventService abilityService)
        {
            _tooltipService = tooltipService;
            _abilityService = abilityService;

            _abilityService.OnAddedAbilityData += AddAbility;
        }

        public void AddAbility(AbilityDataBase data)
        {
            Debug.Log(data.Name);
            HasAbilityItem item = Instantiate(_abilityItemPrefab, _abilityParentTransform);
            item.SetData(data, _tooltipService.ShowTooltip, _tooltipService.CloseTooltip);
        }
    }
}
