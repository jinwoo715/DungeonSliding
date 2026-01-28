using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
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
        private FieldCombatantManager _fieldCombatantManager;

        [SerializeField] private GameSceneManager _gameSceneManager;
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Player _player;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private GameSceneUIManager _gameSceneUIManager;

        private void Start()
        {
            _gameSceneManager.Init(_rewardManager, _mapManager, _player, _enemyManager, _battleManager, _inputSystem, _modeController, _gameSceneUIManager);
            _fieldCombatantManager = new FieldCombatantManager(_enemyManager, _player);
            BindEvent();
            ChildInit();

            _gameSceneManager.PrepareStage();
        }

        private void ChildInit()
        {
            _player.Init();

            _player.SetGameModeChanger(_modeController);
            _player.SetCombatSensor(_fieldCombatantManager);
            _player.SetAttackRequestListener(_battleManager);

            _inputCoordinator.Init(_player);
            
            _enemyManager.WireInterfaces(_mapManager, _battleManager, _fieldCombatantManager);
            _enemyManager.LoadData();

            _mapManager.Init(_player);
            
            _battleManager.Init(_fieldCombatantManager, _modeController);

            _gameSceneUIManager.Init();
        }

        private void BindEvent()
        {
            _inputSystem.OnInputEvnet += _inputCoordinator.OnInputHandle;
            
            _inputCoordinator.IsMoveableFlowFunc += () => _modeController.IsCanMove;
            
            _mapManager.SetEnemyEvent += _enemyManager.SetEnemy;

            _enemyManager.OnEnemyRewardEvent += _rewardManager.GainReward;

            _rewardManager.GetRewardEvent += _player.GetReward;
            
            _player.FinishSlideEvent += _battleManager.StartBattleSequence;
            _player.GetMoveContextFunc += _mapManager.GetMoveContext;
        }

        private void OnDestroy()
        {
            _triggerEventBus.ClearInstance();
        }
    }
}