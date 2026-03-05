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
        private FieldAttackRequesterManager _fieldAttackRequesterManager = new FieldAttackRequesterManager();
        private RouteBuilder _routeBuilder;
        private BossAbilityManager _bossAbilityManager;
        private MoveRule _moveRule = new MoveRule();
        private GameVisualController _visualContoller;
        private EnemyAbilityFactory _enemyAbilityFactory;

        [Header("Camera Controller")]
        [SerializeField] private CameraController cameraController;

        [SerializeField] private Camera cam;
        [SerializeField] private GameObject dirLight;
        [SerializeField] private GameObject playerLight;

        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private GameSceneUIManager _gameSceneUIManager;
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Player _player;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private ObstacleObjectController _obstacleController;

        [SerializeField] private EnemyTooltipClicker _enemyTooltipClicker;

        private void Start()
        {
            _gameSceneManager.Init(_rewardManager, _mapManager, _player, _enemyManager, _battleManager, _inputSystem, _modeController, _gameSceneUIManager, _obstacleController);
            _fieldCombatantManager = new FieldCombatantManager(_enemyManager, _player);
            BindEvent();
            ChildInit();

            _gameSceneManager.ClearFloor();
        }

        private void ChildInit()
        {
            _routeBuilder = new RouteBuilder(_mapManager);
            _abilitySystem = new AbilitySystem(_fieldCombatantManager, _player);
            
            _gameSceneUIManager.Init(_player.StatReadOnly, _combatEventBus, _abilitySystem, _gameSceneManager);

            _player.Initialize(ECreatureType.Player, _battleManager);
            _player.SetData(_routeBuilder, _mapManager, _moveRule);
            _player.SetData(new Stats.CreatureBaseStat(GameManager.Config.Player.HP, GameManager.Config.Player.DMG, GameManager.Config.Player.MVCount));
            _player.RegisterRequester(_fieldAttackRequesterManager.RegisterPlayerAttackRequester, _fieldAttackRequesterManager.UnRegisterPlayerAttackRequester);

            _inputCoordinator.Init(_player);

            _enemyManager.WireInterfaces(_mapManager, _obstacleController, _gameSceneUIManager.EnemyStatUIService, _combatEventBus, _fieldAttackRequesterManager);
            _enemyManager.LoadData();

            _mapManager.Init(_player.Tile);
            
            _battleManager.Init(_fieldCombatantManager, _fieldAttackRequesterManager);

            _rewardManager.Init(_combatEventBus);

            _modeController.Init();

            _enemyTooltipClicker.Initialize(_gameSceneUIManager.EnemyTooltipService);

            _visualContoller = new GameVisualController(cam, dirLight, playerLight, _gameSceneUIManager.EnemyStatUIService);
            _bossAbilityManager = new BossAbilityManager(_fieldCombatantManager, _moveRule, _player.StatReadOnly, _visualContoller);
            _enemyAbilityFactory = new EnemyAbilityFactory(_bossAbilityManager);
        }

        private void BindEvent()
        {
            _obstacleController.Wire(_mapManager);

            _inputSystem.OnInputEvnet += _inputCoordinator.OnInputHandle;
            
            _inputCoordinator.IsMoveableFlowFunc += () => _modeController.IsCanMove;
            
            _mapManager.RequestSpawnEnemyEvent += _enemyManager.SpawnEnemy;
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}