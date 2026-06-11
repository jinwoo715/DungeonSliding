using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stage;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Bootstrap 
{
    public class GameSceneBootstrapper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WorldReferences _world;
        [SerializeField] private PlayerReferences _player;
        [SerializeField] private UIReferences _ui;
        [SerializeField] private BattleManager _battleManager;

        [Header("Managers")]
        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameVisualController _visualController;

        private GameWinPresenter _winPresenter = new GameWinPresenter();
        private GameLoosePresenter _loosePresenter = new GameLoosePresenter();

        [SerializeField] private GameResultViewer _winViewer;
        [SerializeField] private GameResultViewer _looseViewer;

        // 시스템 인스턴스
        private readonly PlayerAbilitySystem _abilitySystem = new();
        private readonly PlayerAbilityContext _playerAbilityContext = new();
        private readonly PlayerAbilityFactory _playerAbilityFactory = new();
        private readonly EnemyAbilityContext _enemyAbilityContext = new();
        private readonly EnemyAbilityFactory _enemyAbilityFactory = new();

        private readonly FieldCombatantFinder _fieldCombatantFinder = new();
        private readonly GameStateController _gameStateController = new();
        private readonly GameTriggerEventBus _triggerEventBus = new();
        private readonly MoveRule _moveRule = new();
        private readonly RouteBuilder _routeBuilder = new();

        private GameEventBinder _eventBinder;

        private void Start()
        {
            AbilityBusyCounter.Clear();
            Initialize();
            _gameSceneManager.ClearFloor();
        }

        private void Initialize()
        {
            // 1. 핵심 시스템 초기화
            _fieldCombatantFinder.Init(_world.EnemyManager, _player.Controller.Player);
            _battleManager.Init(_fieldCombatantFinder, _gameStateController);
            _visualController.Init(_ui.EnemyStatPresenter);
            _gameSceneManager.Init(_gameStateController, _ui.UIManager, _world.StageController,_player.Controller, _ui.UIManager);
            _routeBuilder.Init(_world.MapManager);
            _gameStateController.Init(_routeBuilder, _battleManager);

            _gameSceneManager.Moveable = _player.Controller.Moveable;
            _gameSceneManager.PlayerStat = _player.Controller.StatReadOnly;
            _gameSceneManager.StageViewer = _world.StageController;
            _abilitySystem.OnAddedRuleAbility += _gameSceneManager.AddedPlayerAbility;

            _winPresenter.Init(_winViewer, _gameSceneManager);
            _loosePresenter.Init(_looseViewer, _gameSceneManager);

            // 2. Installer를 이용한 의존성 주입
            WorldInstaller.Install(_world, _player.Controller.Player, _enemyAbilityFactory, GameManager.Data);

            new PlayerInstaller().Install(
                _player, 
                _routeBuilder, 
                _moveRule, 
                _battleManager, 
                _abilitySystem,
                _world.MapManager,
                _battleManager);

            var abilityInstaller = new AbilityInstaller();
            abilityInstaller.InstallPlayerAbility(
                _abilitySystem,
                _playerAbilityFactory,
                _playerAbilityContext, 
                _player.Controller, 
                _fieldCombatantFinder, 
                _routeBuilder);
            
            abilityInstaller.InstallEnemyAbility(
                _enemyAbilityFactory, 
                _enemyAbilityContext, 
                _fieldCombatantFinder, 
                _moveRule, 
                _player.Controller, 
                _visualController, 
                _routeBuilder);

            new UIInstaller().Install(
                _ui, 
                _world.StageController,
                _world.StageController, 
                _abilitySystem, 
                _player.Controller);

            // 3. 이벤트 바인딩
            _eventBinder = new GameEventBinder();
            _eventBinder.Bind(
                _world.EnemyManager, 
                _world.MapManager, 
                _ui.EnemyStatPresenter, 
                _player.Controller.Moveable, 
                _battleManager, 
                _abilitySystem, 
                _gameStateController,
                _player.InputCoordinator, 
                _player.Controller
            );
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
            _eventBinder?.Dispose();
            _gameStateController.Clear();
        }
    }
}
