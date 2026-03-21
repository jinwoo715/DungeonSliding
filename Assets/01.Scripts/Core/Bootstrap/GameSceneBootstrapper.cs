using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.UI;
using UnityEngine;

   
namespace JW.DungeonSliding.GamePlay.Bootstrap 
{
    public class GameSceneBootstrapper : MonoBehaviour
    {
        private AbilitySystem _abilitySystem = new AbilitySystem();
        private CombatEventBus _combatEventBus = new();
        private EnemyAbilityManager _enemyAbilityManager;
        private EnemyAbilityFactory _enemyAbilityFactory;
        private FieldCombatantManager _fieldCombatantManager;
        private GameVisualController _visualContoller;
        private GameSequenceController _modeController = new GameSequenceController();
        private GameTriggerEventBus _triggerEventBus = new GameTriggerEventBus();
        private InputCoordinator _inputCoordinator = new InputCoordinator();
        private LevelSystem _leveling = new LevelSystem();
        private MoveRule _moveRule = new MoveRule();
        private RewardManager _rewardManager = new RewardManager();
        private RouteBuilder _routeBuilder = new RouteBuilder();

        private PlayerAbilityContext _playerAbilityContext = new PlayerAbilityContext();

        [Header("Camera Controller")]
        [SerializeField] private BattleManager _battleManager;

        [SerializeField] private CameraController cameraController;
        
        [SerializeField] private Camera cam;
        
        [SerializeField] private EnemyManager _enemyManager;
        
        [SerializeField] private GameObject dirLight;
        [SerializeField] private GameObject playerLight;

        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameSceneUIManager _gameSceneUIManager;
        
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private ObstacleObjectController _obstacleController;
        [SerializeField] private EnemyTooltipClicker _enemyTooltipClicker;



        public FunctionTester functionTester;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _playerController.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -5));
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                _playerController.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, -5));
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                _leveling.LevelUp();
            }
        }


        private void Start()
        {
            AbilityBusyCounter.Clear();

            _gameSceneManager.Init(_rewardManager, _mapManager, _playerController.Player, _enemyManager, _battleManager, _inputSystem, _modeController, _gameSceneUIManager, _obstacleController);
            _fieldCombatantManager = new FieldCombatantManager(_enemyManager, _playerController.Player);
            BindEvent();
            ChildInit();

            _gameSceneManager.ClearFloor();
            _leveling.Initialize();

            functionTester.Init(_abilitySystem);
        }

        private void ChildInit()
        {
            _playerAbilityContext.SetOwner(_playerController.Player);
            _playerAbilityContext.Register<ICombatantSensor>(_fieldCombatantManager);
            _playerAbilityContext.Register<IRouteService>(_routeBuilder);

            _abilitySystem.Init(_playerAbilityContext, _leveling);
  
            _routeBuilder.Init(_mapManager);

            _gameSceneUIManager.Init(_playerController.Player, _leveling, _playerController.NextAttackEnhancer, _combatEventBus, _abilitySystem, _gameSceneManager);

            _playerController.InitializePlayer(_routeBuilder, _moveRule, _battleManager, _leveling, _abilitySystem);
            _playerController.RegisterContext(_playerAbilityContext);

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

            _abilitySystem.OnExcuteAbilitySelection += _ => { _modeController.EnterGameMode(EGameModeType.AbilityUI); };
            _abilitySystem.OnSelectAbility += _ => { _modeController.ExitGameMode(EGameModeType.AbilityUI); };
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}