using UnityEngine;
using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.UI
{
    public class AbilityUIController : MonoBehaviour
    {
        private AbilitySession _currentSession;

        [SerializeField] private AbilitySelectView _abilitySelectView;

        public void Init()
        {
            _abilitySelectView.Init();
            _abilitySelectView.Bind(SelectAbility, Reroll);
        }

        public void OpenSelectAbilityView(AbilitySession session)
        {
            _currentSession = session;
            _abilitySelectView.SetAilityDatas(_currentSession.Abilities, _currentSession.RerollCount);

            _abilitySelectView.gameObject.SetActive(true);
        }

        public void Reroll()
        {
            if (_currentSession.TryRerollAbilities())
            {
                _abilitySelectView.SetAilityDatas(_currentSession.Abilities, _currentSession.RerollCount);
            }
        }

        public void SelectAbility(int abilityUid)
        {
            _currentSession.SelectAbiltyUIDEvent(abilityUid);
            _abilitySelectView.gameObject.SetActive(false);
            _currentSession = null;
        }
    }
}
