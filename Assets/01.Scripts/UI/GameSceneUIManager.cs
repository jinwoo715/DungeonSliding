using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
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
        [SerializeField] private EnemyStatUIManager _enemyStatUIManager;
        [SerializeField] private HitDamageViewer _hitDamageViewer;

        [Header("Presenter")]
        [SerializeField] private GameTooltipPresenter _tooltipPresenter;
        [SerializeField] private HasAbilityPresenter _hasAbilityPresenter;
        [SerializeField] private PlayerStatPresenter _playerStatPresenter;

        [Header("Viewer")]
        [SerializeField] private ActViewer _actViewer;

        public IEnemyStatUIService EnemyStatUIService => _enemyStatUIManager;

        public void Init(IPlayerStatReader statReadOnly, ICombatEventPresenter combatEventPresenter, IAbilityService abilityService, IActService actService)
        {
            _abilityUIController.Initialize(abilityService);
            _playerStatPresenter.Init(statReadOnly);
            _enemyStatUIManager.Init();
            _hitDamageViewer.Init(combatEventPresenter);

            _hasAbilityPresenter.Initialize(_tooltipPresenter, abilityService);
            _actViewer.Initialize(actService);
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
