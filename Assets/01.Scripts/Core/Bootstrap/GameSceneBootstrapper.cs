using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;
using UnityEngine;

   
namespace JW.DungeonSliding.GamePlay.Bootstrap 
{

    public class GameSceneBootstrapper : MonoBehaviour
    {
        private GameSceneManager _gameSceneManager;
        private RewardManager _rewardManager = new RewardManager();
        private GameModeController _modeController = new GameModeController();
        private InputCoordinator _inputCoordinator = new InputCoordinator();
        private FieldCombatantSensor _combatantSensor;

        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Player _player;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;

        private void Start()
        {
            _gameSceneManager = new GameSceneManager(_rewardManager, _mapManager, _player, _enemyManager, _battleManager, _inputSystem);
            _combatantSensor = new FieldCombatantSensor(_enemyManager, _player);
            BindEvent();
            ChildInit();

            _gameSceneManager.StartStage();
        }

        private void ChildInit()
        {
            _player.Init();
            _player.Init(_modeController);
            _player._attackRequestListener = _battleManager;

            _inputCoordinator.Init(_player);
            
            _enemyManager.Init(_mapManager, _battleManager, _combatantSensor);

            _mapManager.Init(_player);
            
            _battleManager.Init(_enemyManager, _player, _modeController);
        }

        private void BindEvent()
        {
            _inputCoordinator.IsMoveableFlowFunc = () => _modeController.IsCanMove;

            _inputSystem.OnInputEvnet = _inputCoordinator.OnInputHandle;

            _mapManager.SetEnemyEvent += _enemyManager.SetEnemy;

            _rewardManager.GetRewardEvent = _player.GetReward;

            _player.FinishSlideEvent += _battleManager.StartBattleSequence;
            _player.GetMoveContextFunc = _mapManager.GetMoveContext;
            _player._sensor = _combatantSensor;
        }
    }
}