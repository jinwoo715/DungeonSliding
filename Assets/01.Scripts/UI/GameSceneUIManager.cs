using JW.DungeonSliding.GamePlay;
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
        [SerializeField] private HitDamageViewer _hitDamageViewer;

        [Header("Presenter")]
        [SerializeField] private GameTooltipPresenter _abilityTooltipPresenter;
        [SerializeField] private GameTooltipPresenter _enemyTooltipPresenter;

        [SerializeField] private EnemyStatPresenter _enemyStatUIManager;
        [SerializeField] private PlayerStatPresenter _playerStatPresenter;
        [SerializeField] private HasAbilityPresenter _hasAbilityPresenter;

        [Header("Viewer")]
        [SerializeField] private ActViewer _actViewer;

        public IEnemyStatUIService EnemyStatUIService => _enemyStatUIManager;
        public ITooltipService EnemyTooltipService => _enemyTooltipPresenter;

        public void Init(ICombatant _player, ILevelProgress levelProgress, INextAttackEnhancer nextAttackEnhancer, ICombatEventPresenter combatEventPresenter, IAbilityEventService abilityService, IActService actService)
        {
            _abilityUIController.Initialize(abilityService);
            _playerStatPresenter.Init(_player.StatReadOnly, _player.StatModifier, levelProgress, nextAttackEnhancer);
            _enemyStatUIManager.Init();
            _hitDamageViewer.Init();

            _hasAbilityPresenter.Initialize(_abilityTooltipPresenter, abilityService);
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
        public void SetAbilitySession(AbilitySelectSession session)
        {
            _abilityUIController.OpenSelectAbilityView(session);
        }
    }
}
