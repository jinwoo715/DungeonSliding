using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
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
        [SerializeField] private PlayerStatUIContoller _playerStatUI;
        [SerializeField] private EnemyStatUIManager _enemyStatUIManager;
        [SerializeField] private HitDamageViewer _hitDamageViewer;

        public IEnemyStatUIService EnemyStatUIService => _enemyStatUIManager;

        public void Init(GamePlay.Stats.IPlayerStatProvider statReadOnly, ICombatEventPresenter combatEventPresenter)
        {
            _abilityUIController.Init();
            _playerStatUI.Init(statReadOnly);
            _enemyStatUIManager.Init();
            _hitDamageViewer.Init(combatEventPresenter);
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
