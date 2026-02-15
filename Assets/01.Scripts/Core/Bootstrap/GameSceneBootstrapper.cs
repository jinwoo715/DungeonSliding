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
        private BossAbilityManager _bossAbilityManager;
        private MoveRule _moveRule = new MoveRule();
        private GameVisualController _visualContoller;
        private EnemyAbilityFactory _enemyAbilityFactory;

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

        private void Start()
        {
            _gameSceneManager.Init(_rewardManager, _mapManager, _player, _enemyManager, _battleManager, _inputSystem, _modeController, _gameSceneUIManager, _obstacleController);
            _fieldCombatantManager = new FieldCombatantManager(_enemyManager, _player);
            BindEvent();
            ChildInit();

            _gameSceneManager.PrepareStage();
        }

        private void ChildInit()
        {
            _routeBuilder = new RouteBuilder(_mapManager);
            _abilitySystem = new AbilitySystem(_gameSceneUIManager, _fieldCombatantManager, _player);
            
            _gameSceneUIManager.Init(_player, _combatEventBus);

            _player.Init(_combatEventBus, ECretureType.Player);

            PlayerData player = new PlayerData(GameManager.Config.Player.HP, GameManager.Config.Player.DMG, GameManager.Config.Player.MVCount);

            _player.SetData(player, _routeBuilder, _mapManager, _moveRule);

            _inputCoordinator.Init(_player);


            _enemyManager.WireInterfaces(_mapManager, _obstacleController, _gameSceneUIManager.EnemyStatUIService, _combatEventBus);
            _enemyManager.LoadData();

            _mapManager.Init(_player);
            
            _battleManager.Init(_fieldCombatantManager);

            _rewardManager.Init(_combatEventBus);

            _modeController.Init();
            _visualContoller = new GameVisualController(cam, dirLight, playerLight, _gameSceneUIManager.EnemyStatUIService);
            _bossAbilityManager = new BossAbilityManager(_fieldCombatantManager, _moveRule, _player, _visualContoller);
            _enemyAbilityFactory = new EnemyAbilityFactory(_bossAbilityManager);
        }

        private void BindEvent()
        {
            _obstacleController.Wire(_mapManager);

            _inputSystem.OnInputEvnet += _inputCoordinator.OnInputHandle;
            
            _inputCoordinator.IsMoveableFlowFunc += () => _modeController.IsCanMove;
            
            _mapManager.SetEnemyEvent += _enemyManager.SetEnemy;
            _mapManager.RequestSpawnEnemyEvent += _enemyManager.SpawnEnemy;
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}