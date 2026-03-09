using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.UI;
using UnityEngine;

   
namespace JW.DungeonSliding.GamePlay.Bootstrap 
{
    
    
    public class GameSceneBootstrapper : MonoBehaviour
    {
        private RewardManager _rewardManager = new RewardManager();
        private GameModeController _modeController = new GameModeController();
        private InputCoordinator _inputCoordinator = new InputCoordinator();
        private GameTriggerEventBus _triggerEventBus = new GameTriggerEventBus();
        private CombatEventBus _combatEventBus = new();
        private AbilitySystem _abilitySystem;
        private FieldCombatantManager _fieldCombatantManager;
        private RouteBuilder _routeBuilder;
        private EnemyAbilityManager _enemyAbilityManager;
        private MoveRule _moveRule = new MoveRule();
        private GameVisualController _visualContoller;
        private EnemyAbilityFactory _enemyAbilityFactory;
        private LevelSystem _leveling = new LevelSystem();

        private PlayerAbilityContext _playerAbilityContext = new PlayerAbilityContext();

        [Header("Camera Controller")]
        [SerializeField] private CameraController cameraController;

        [SerializeField] private Camera cam;
        [SerializeField] private GameObject dirLight;
        [SerializeField] private GameObject playerLight;

        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameSceneUIManager _gameSceneUIManager;
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private ObstacleObjectController _obstacleController;

        [SerializeField] private EnemyTooltipClicker _enemyTooltipClicker;

        private void Start()
        {
            AbilityBusyCounter.Clear();

            _gameSceneManager.Init(_rewardManager, _mapManager, _playerController.Player, _enemyManager, _battleManager, _inputSystem, _modeController, _gameSceneUIManager, _obstacleController);
            _fieldCombatantManager = new FieldCombatantManager(_enemyManager, _playerController.Player);
            BindEvent();
            ChildInit();

            _gameSceneManager.ClearFloor();
        }

        private void ChildInit()
        {
            _leveling.Initialize();

            _playerAbilityContext.SetOwner(_playerController.Player);
            _playerAbilityContext.Register<IMoveable>(_playerController.Moveable);
            _playerAbilityContext.Register<ICombatantSensor>(_fieldCombatantManager);

            _abilitySystem.Init(_playerAbilityContext, _leveling, _playerController.AbilityRegister);

            _routeBuilder = new RouteBuilder(_mapManager);

            _gameSceneUIManager.Init(_playerController.Player, _leveling, _combatEventBus, _abilitySystem, _gameSceneManager);

            _playerController.InitializePlayer(_routeBuilder, _moveRule, _battleManager, _leveling);

            _inputCoordinator.Init(_playerController.Moveable);

            _enemyManager.WireInterfaces(_mapManager, _obstacleController, _gameSceneUIManager.EnemyStatUIService, _combatEventBus, _battleManager);
            _enemyManager.LoadData();

            _mapManager.Init(_playerController.Tile);
            
            _battleManager.Init(_fieldCombatantManager);

            _rewardManager.Init(_combatEventBus);

            _modeController.Init();

            _enemyTooltipClicker.Initialize(_gameSceneUIManager.EnemyTooltipService);

            _visualContoller = new GameVisualController(cam, dirLight, playerLight, _gameSceneUIManager.EnemyStatUIService);
            _enemyAbilityManager = new EnemyAbilityManager(_fieldCombatantManager, _moveRule, _playerController.StatReadOnly, _visualContoller);
            _enemyAbilityFactory = new EnemyAbilityFactory(_enemyAbilityManager);
        }

        private void BindEvent()
        {
            _obstacleController.Wire(_mapManager);

            _inputSystem.OnInputEvnet += _inputCoordinator.OnInputHandle;
            
            _inputCoordinator.IsMoveableFlowFunc += () => _modeController.IsCanMove;
            
            _mapManager.RequestSpawnEnemyEvent += _enemyManager.SpawnEnemy;

            _playerController.Moveable.OnMoveEnd += _battleManager.StartBattleSequence;
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}