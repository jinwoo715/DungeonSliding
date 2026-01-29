using JW.DungeonSliding.GamePlay.Ability;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public interface IUIFader
    {
        IEnumerator FadeOut();
        IEnumerator FadeIn();
    }

    public class GameSceneUIManager : MonoBehaviour, IUIFader, IAbilitySelectService
    {
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private AbilityUIController _abilityUIController;

        public void Init()
        {
            _abilityUIController.Init();
        }

        public IEnumerator FadeIn()
        {
            yield return _fadeController.CoFadeIn();
        }

        public IEnumerator FadeOut()
        {
            yield return _fadeController.CoFadeOut();
        }
        public void SetAbilitySession(AbilitySession session)
        {
            _abilityUIController.OpenSelectAbilityView(session);
        }
    }
}
