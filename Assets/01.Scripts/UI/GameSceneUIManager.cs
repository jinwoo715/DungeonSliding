using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stage;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public interface IUIFader
    {
        IEnumerator FadeOut();
        IEnumerator FadeIn();
    }

    public class GameSceneUIManager : MonoBehaviour, IUIFader, IAbilitySelectService, IPopupService
    {
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private AbilitySelectPresenter _abilityUIController;
        [SerializeField] private HitDamageViewer _hitDamageViewer;

        [Header("Presenter")]
        [SerializeField] private GamePopupPresenter _gamePopupPresenter;

        [SerializeField] private HasAbilityPresenter _hasAbilityPresenter;

        public void Init()
        {
            _hitDamageViewer.Init();
            _fadeController.SetAlpha(1);
            _gamePopupPresenter.Init();
        }

        public IEnumerator FadeIn()
        {
            yield return _fadeController.CoFadeIn();
        }
        public IEnumerator FadeOut()
        {
            yield return _fadeController.CoFadeOut();
        }
        public void SetAbilitySession(AbilitySelectSession session)
        {
            _abilityUIController.OpenSelectAbilityView(session);
        }

        public void ShowOneButtonPopup(string name, string desc, ButtonSet buttonSet)
        {
            Debug.Log("Á³´Ù! ÆË¾÷!");
            _gamePopupPresenter.ShowOneButtonPopup(name, desc, buttonSet);
        }
    }
}
