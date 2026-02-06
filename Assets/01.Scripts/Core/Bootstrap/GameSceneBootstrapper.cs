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
            _abilitySystem = new AbilitySystem(_gameSceneUIManager, _modeController, _player);
            
            _gameSceneUIManager.Init(_player, _combatEventBus);

            _player.Init(_combatEventBus, ECretureType.Player);
            _player.SetData(GameManager.Instance.Resource.PlayerData, _routeBuilder, _mapManager);

            _inputCoordinator.Init(_player);
            
            _enemyManager.WireInterfaces(_mapManager, _obstacleController, _gameSceneUIManager.EnemyStatUIService, _combatEventBus);
            _enemyManager.LoadData();

            _mapManager.Init(_player);
            
            _battleManager.Init(_fieldCombatantManager);

            _rewardManager.Init(_combatEventBus);

            _modeController.Init();
        }

        private void BindEvent()
        {
            _obstacleController.Wire(_mapManager);

            _inputSystem.OnInputEvnet += _inputCoordinator.OnInputHandle;
            
            _inputCoordinator.IsMoveableFlowFunc += () => _modeController.IsCanMove;
            
            _mapManager.SetEnemyEvent += _enemyManager.SetEnemy;
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}