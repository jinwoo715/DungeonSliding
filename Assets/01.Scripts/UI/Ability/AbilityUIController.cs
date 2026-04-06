using UnityEngine;
using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.UI
{
    public class AbilityUIController : MonoBehaviour
    {
        private AbilitySelectSession _currentSession;

        [SerializeField] private AbilitySelectView _abilitySelectView;

        public void Init(IAbilityEventService abilityService)
        {
            _abilitySelectView.Init();
            _abilitySelectView.Bind(SelectAbility, Reroll);

            abilityService.OnExcuteAbilitySelection += OpenSelectAbilityView;
        }

        public void OpenSelectAbilityView(AbilitySelectSession session)
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
    }
}
