using JW.DungeonSliding;
using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stage;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.UI;
using System;
using UnityEngine;


[System.Serializable]
 public class WorldReferences
{
    public MapManager MapManager;
    public StageController StageController;
    public EnemyManager EnemyManager;
    public ObstacleObjectController ObstacleController;
}

public static class WorldInstaller
{
    public static void Install(WorldReferences world, ICombatant player, EnemyAbilityFactory enemyAbilityFactory)
    {
        world.MapManager.Init();

        world.StageController.Init(world.MapManager, world.ObstacleController, player.Tile, world.EnemyManager);

        world.ObstacleController.Init(world.MapManager);

        world.EnemyManager.Init(world.ObstacleController, enemyAbilityFactory);
    }
}

[System.Serializable]
public class PlayerReferences
{
    public PlayerController Controller;
    public InputCoordinator InputCoordinator;
}

public class PlayerInstaller
{
    public void Install(PlayerReferences player, IRouteService routeService, IMoveRule moveRule, IAttackRegister requesterRegistry, IAbilityEventService abilityEventService)
    {
        player.InputCoordinator.Init();
        player.Controller.Init(routeService, moveRule, requesterRegistry, abilityEventService);
    }
}

public class AbilityInstaller
{
    public void InstallPlayerAbility(
        PlayerAbilitySystem abilitySystem,
        PlayerAbilityContext context,
        PlayerController player,
        FieldCombatantFinder finder,
        RouteBuilder routeBuilder)
    {
        player.RegisterContext(context);
        context.SetOwner(player.Player);
        context.Register<ICombatantSensor>(finder);
        context.Register<IRouteService>(routeBuilder);

        abilitySystem.Init(context, player.Level);
    }

    public void InstallEnemyAbility(
        EnemyAbilityFactory factory,
        EnemyAbilityContext context,
        FieldCombatantFinder finder,
        MoveRule moveRule,
        PlayerController player,
        GameVisualController visualController,
        RouteBuilder routeBuilder)
    {
        context.Register<ICombatantSensor>(finder);
        context.Register<IMoveRule>(moveRule);
        context.Register<IStatReadOnly>(player.StatReadOnly);
        context.Register<IVisualController>(visualController);
        context.Register<IRouteService>(routeBuilder);

        factory.Init(context);
    }
}


[System.Serializable]
public class UIReferences
{
    public GameSceneUIManager UIManager;

    [Header("Controller")]
    public AbilityUIController AbilityUIController;
    public EnemyTooltipController EnemyTooltipClicker;

    [Header("Presenter")]
    public HasAbilityPresenter HasAbilityPresenter;
    public EnemyStatPresenter EnemyStatPresenter;
    public PlayerStatPresenter PlayerStatPresenter;
    public GameTooltipPresenter AbilityTooltipPresenter;
    public GameTooltipPresenter EnemyTooltipPresenter;

    [Header("Viewer")]
    public StageViewer StageViewer;
}

public class UIInstaller
{
    public void Install(UIReferences ui, IStageViewer stageViewer, IAbilityEventService abilityService, IPlayerInfoViewer playerInfoViewer)
    {
        ui.HasAbilityPresenter.Init(ui.AbilityTooltipPresenter, abilityService);
        ui.UIManager.Init();
        ui.StageViewer.Init(stageViewer.TotalFloor, stageViewer.BossFloors);
        ui.AbilityUIController.Init(abilityService);
        ui.PlayerStatPresenter.Init(playerInfoViewer.GetPlayerInfo());
        ui.EnemyTooltipClicker.Init(ui.EnemyTooltipPresenter);
    }
}

public class GameEventBinder : IDisposable
{
    // 해제를 위해 이벤트 발행자(Publisher)들의 참조를 들고 있어야 합니다.
    private IEnemySpawnService _enemySpawnService;
    private IMoveable _moveable;
    private IAbilityEventService _abilityEventService;
    private IInputService _inputService;
    private IGameModeModifier _gameModeModifier;
    private IStageService _stageViewer;

    // 로직 처리를 위한 참조
    private IBoard _board;
    private IEnemyStatUIService _enemyStatUI;
    private IAttackRegister _attackRegister;
    private BattleManager _battleManager;
    private PlayerController _playerController;
    private StageViewer _viewer;

    public void Bind(IEnemySpawnService enemySpawnService, IBoard board,
        IEnemyStatUIService enemyStatUI, 
        IMoveable moveable, BattleManager battleManager,
        IAbilityEventService abilityEventService, IGameModeModifier gameModeModifier,
        IInputService inputService, PlayerController playerController,
        IStageService stageViewer, StageViewer viewer)
    {
        // 1. 참조 저장
        _enemySpawnService = enemySpawnService;
        _board = board;
        _enemyStatUI = enemyStatUI;
        _attackRegister = battleManager;
        _moveable = moveable;
        _battleManager = battleManager;
        _abilityEventService = abilityEventService;
        _gameModeModifier = gameModeModifier;
        _inputService = inputService;
        _playerController = playerController;
        _stageViewer = stageViewer;
        _viewer = viewer;

        _enemySpawnService.OnSpawnEnemy += OnEnemySpawned;
        _enemySpawnService.OnDespawnEnemy += OnEnemyDespawned;

        moveable.OnMoveEnd += battleManager.StartBattleSequence;

        _abilityEventService.OnExcuteAbilitySelection += OnExecuteAbility;
        _abilityEventService.OnSelectAbility += OnSelectAbility;

        inputService.OnMoveInput += playerController.OnPlayerMove;

        gameModeModifier.OnChangeMoveState += playerController.OnChangeMoveState;

        stageViewer.OnChangeFloorEvent += viewer.UpdateFloor;
    }

    // 람다식을 대체하는 기명 메서드들
    private void OnEnemySpawned(Tile tile, Enemy enemy)
    {
        _board.RegisterEnemyTile(tile);
        _enemyStatUI.Attach(enemy.StatUITransform, enemy);
        _attackRegister.RegisterAttackRequester(enemy.AttackRequester, (int)ECreatureType.Enemy);
    }

    private void OnEnemyDespawned(Tile tile, Enemy enemy)
    {
        _board.UnRegisterEnemyTile(tile);
        _enemyStatUI.Detach(enemy);
        _attackRegister.UnRegisterAttackRequester(enemy.AttackRequester, (int)ECreatureType.Enemy);
    }

    public void Dispose()
    {
        if (_enemySpawnService != null)
        {
            _enemySpawnService.OnSpawnEnemy -= OnEnemySpawned;
            _enemySpawnService.OnDespawnEnemy -= OnEnemyDespawned;
        }
        if (_moveable != null) _moveable.OnMoveEnd -= _battleManager.StartBattleSequence;
        if (_abilityEventService != null)
        {
            _abilityEventService.OnExcuteAbilitySelection -= OnExecuteAbility;
            _abilityEventService.OnSelectAbility -= OnSelectAbility;
        }
        if (_inputService != null) _inputService.OnMoveInput -= _playerController.OnPlayerMove;
        if (_gameModeModifier != null) _gameModeModifier.OnChangeMoveState -= _playerController.OnChangeMoveState;
        if (_stageViewer != null) _stageViewer.OnChangeFloorEvent -= _viewer.UpdateFloor;
    }
    private void OnExecuteAbility(AbilitySelectSession ctx) => _gameModeModifier.EnterGameMode(EGameModeType.AbilityUI);
    private void OnSelectAbility(IAbility ctx) => _gameModeModifier.ExitGameMode(EGameModeType.AbilityUI);
}

namespace JW.DungeonSliding.GamePlay.Bootstrap 
{
    public class GameSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private WorldReferences _world;
        [SerializeField] private PlayerReferences _player; // 누락되었던 필드 추가
        [SerializeField] private UIReferences _ui;         // 누락되었던 필드 추가

        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] GameVisualController _visualController;

        // 시스템 객체들
        private PlayerAbilitySystem _abilitySystem = new PlayerAbilitySystem();
        private PlayerAbilityContext _playerAbilityContext = new PlayerAbilityContext();
        private EnemyAbilityContext _enemyAbilityContext = new EnemyAbilityContext();
        private EnemyAbilityFactory _enemyAbilityFactory = new EnemyAbilityFactory();
        private FieldCombatantFinder _fieldCombatantManager = new FieldCombatantFinder();
        private GameSequenceController _modeController = new GameSequenceController();
        private GameTriggerEventBus _triggerEventBus = new GameTriggerEventBus();
        private MoveRule _moveRule = new MoveRule();
        private RouteBuilder _routeBuilder = new RouteBuilder();
        public FieldCombatantFinder FieldConbatantFinder = new FieldCombatantFinder();
        public BattleManager BattleManager = new BattleManager();


        private GameEventBinder _eventBinder; // 바인더 인스턴스

        private void Start()
        {
            AbilityBusyCounter.Clear();
            ChildInit();
            _gameSceneManager.ClearFloor();
        }

        private void ChildInit()
        {
            // 1. 공통 시스템 초기화
            FieldConbatantFinder.Init(_world.EnemyManager, _player.Controller.Player);
            BattleManager.Init(FieldConbatantFinder);
            _visualController.Init(_ui.EnemyStatPresenter);
            _gameSceneManager.Init(_modeController, _ui.UIManager, _world.StageController);
            _routeBuilder.Init(_world.MapManager);
            _modeController.Init(_routeBuilder, BattleManager);

            // 2. Installer들을 통한 하위 도메인 조립 (매우 중요!)
            WorldInstaller.Install(_world, _player.Controller.Player, _enemyAbilityFactory);

            new PlayerInstaller().Install(_player, _routeBuilder, _moveRule, BattleManager, _abilitySystem);

            var abilityInstaller = new AbilityInstaller();
            abilityInstaller.InstallPlayerAbility(_abilitySystem, _playerAbilityContext, _player.Controller, _fieldCombatantManager, _routeBuilder);
            abilityInstaller.InstallEnemyAbility(_enemyAbilityFactory, _enemyAbilityContext, _fieldCombatantManager, _moveRule, _player.Controller, _visualController, _routeBuilder);

            new UIInstaller().Install(_ui, _world.StageController, _abilitySystem, _player.Controller);

            // 3. 마지막으로 모든 이벤트를 묶어줌
            _eventBinder = new GameEventBinder();
            _eventBinder.Bind(
                _world.EnemyManager, _world.MapManager, _ui.EnemyStatPresenter, 
                _player.Controller.Moveable, BattleManager, _abilitySystem, _modeController,
                _player.InputCoordinator, _player.Controller, _world.StageController, _ui.StageViewer
            );
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();

            // 씬 파괴 시 바인더의 Dispose를 호출하여 완벽하게 메모리 누수 방지
            _eventBinder?.Dispose();
        }
    }
}