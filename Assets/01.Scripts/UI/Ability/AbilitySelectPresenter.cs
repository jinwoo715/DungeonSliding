using UnityEngine;
using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.UI
{
    public class AbilitySelectPresenter : MonoBehaviour
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

            SetAbilityData();

            _abilitySelectView.gameObject.SetActive(true);
        }

        private void SetAbilityData()
        {
            _abilitySelectView.SetAilityDatas(_currentSession.SelectableAbilities, _currentSession.RerollCount);
        }

        public void Reroll()
        {
            if (_currentSession.TryRerollAbilities())
            {
                SetAbilityData();
            }
        }
        public void SelectAbility(AbilityDataBase abilityData)
        {
            _abilitySelectView.gameObject.SetActive(false);
            _currentSession.SelectAbiltyUIDEvent(abilityData);
            _currentSession.Clear();
        }
    }
}
