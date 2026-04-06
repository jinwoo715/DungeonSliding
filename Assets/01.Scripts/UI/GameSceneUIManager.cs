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

    public class GameSceneUIManager : MonoBehaviour, IUIFader, IAbilitySelectService
    {
        [SerializeField] private FadeController _fadeController;
        [SerializeField] private AbilityUIController _abilityUIController;
        [SerializeField] private HitDamageViewer _hitDamageViewer;

        [Header("Presenter")]
        

        [SerializeField] private HasAbilityPresenter _hasAbilityPresenter;

        public void Init()
        {
            _hitDamageViewer.Init();
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
    }
}
