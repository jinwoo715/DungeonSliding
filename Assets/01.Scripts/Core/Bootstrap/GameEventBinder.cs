using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stage;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.UI;
using System;

namespace JW.DungeonSliding.GamePlay.Bootstrap
{
    public class GameEventBinder : IDisposable
    {
        private IEnemySpawnService _enemySpawnService;
        private IMoveable _moveable;
        private IAbilityEventService _abilityEventService;
        private IInputService _inputService;
        private IGameStateModifier _gameModeModifier;
        private IStageService _stageViewer;

        private IBoard _board;
        private IEnemyStatUIService _enemyStatUI;
        private IAttackRegister _attackRegister;
        private BattleManager _battleManager;
        private PlayerController _playerController;
        private StageViewer _viewer;

        public void Bind(IEnemySpawnService enemySpawnService, IBoard board,
            IEnemyStatUIService enemyStatUI,
            IMoveable moveable, BattleManager battleManager,
            IAbilityEventService abilityEventService, IGameStateModifier gameModeModifier,
            IInputService inputService, PlayerController playerController,
            IStageService stageViewer, StageViewer viewer)
        {
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
            _enemySpawnService.OnEnemyDeath += OnEnemyDie;

            moveable.OnMoveEnd += battleManager.StartBattleSequence;

            _abilityEventService.OnExcuteAbilitySelection += OnExecuteAbility;
            _abilityEventService.OnSelectAbility += OnSelectAbility;

            inputService.OnMoveInput += playerController.OnPlayerMove;

            gameModeModifier.OnChangeMoveState += playerController.OnChangeMoveState;

            stageViewer.OnChangeFloorEvent += viewer.UpdateFloor;
        }

        private void OnEnemySpawned(Tile tile, Enemy enemy)
        {
            _board.RegisterEnemyTile(tile);
            _enemyStatUI.Attach(enemy.StatUITransform, enemy);
            _attackRegister.RegisterAttackRequester(enemy.AttackRequester, (int)ECreatureType.Enemy);
        }

        private void OnEnemyDie(Tile tile, Enemy enemy)
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
                _enemySpawnService.OnEnemyDeath -= OnEnemyDie;
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

        private void OnExecuteAbility(AbilitySelectSession ctx) => _gameModeModifier.EnterGameState(EGameStateType.AbilityUI);
        private void OnSelectAbility(IAbility ctx) => _gameModeModifier.ExitGameState(EGameStateType.AbilityUI);
    }
}
