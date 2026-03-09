using UnityEngine;
using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.UI
{
    public class AbilityUIController : MonoBehaviour
    {
        private AbilitySession _currentSession;
        private IAbilityEventService _abilityService;

        [SerializeField] private AbilitySelectView _abilitySelectView;

        public void Initialize(IAbilityEventService abilityService)
        {
            _abilitySelectView.Init();
            _abilitySelectView.Bind(SelectAbility, Reroll);

            _abilityService = abilityService;

            _abilityService.OnExcuteAbilitySelection += OpenSelectAbilityView;
        }

        public void OpenSelectAbilityView(AbilitySession session)
        {
            _currentSession = session;
            _abilitySelectView.SetAilityDatas(_currentSession.SelectableAbilities, _currentSession.RerollCount);

            _abilitySelectView.gameObject.SetActive(true);
        }

        public void Reroll()
        {
            if (_currentSession.TryRerollAbilities())
            {
                _abilitySelectView.SetAilityDatas(_currentSession.SelectableAbilities, _currentSession.RerollCount);
            }
        }

        public void SelectAbility(AbilityDataBase abilityData)
        {
            _currentSession.SelectAbiltyUIDEvent(abilityData);
            _abilitySelectView.gameObject.SetActive(false);
            _currentSession = null;
        }

        private void OnDisable()
        {
            _abilityService.OnExcuteAbilitySelection -= OpenSelectAbilityView;
        }
    }
}
