using UnityEngine;
using JW.Utility;
using System.Collections.Generic;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.Core;

namespace JW.DungeonSliding.GamePlay.Context
{
    public class GameSceneContext
    {
        private static GameSceneContext _instance;
        public static GameSceneContext Instance => _instance;

        public RewardManager PlayerReward { get; }
        public MapManager _mapManager;
        public Player _player;
        public EnemyManager _enemyManager;
        public BattleManager _battleManager;
        public InputSystem _inputSystem;

        [SerializeField] private List<MapData> _mapDatas;
        private ShuffleBag<MapData> _mapBag;
        private EGameFlowType _gameFlowType = EGameFlowType.None;
        public RewardManager Reward => PlayerReward;
        public int Floor { get; private set; }

        public GameSceneContext(RewardManager reward, MapManager map, Player player, EnemyManager enemyManager, BattleManager battleManager, InputSystem input)
        {
            PlayerReward = reward;
            _mapManager = map;
            _player = player;
            _enemyManager = enemyManager;
            _battleManager = battleManager;
            _inputSystem = input;

            _instance = this;
        }
        private void Init()
        {
            _mapBag = new ShuffleBag<MapData>(GameManager.Instance.Resource.MapData);

            BindEvent();
            ChildInit();

            _mapManager.SetMap(_mapBag.GetItem(), _player);
        }
        private MapData GetRandomMap()
        {
            return null;
        }
        private void ChildInit()
        {
            _player.Init();
            _player.SetCretureStat(new CretureStat(ConstData.PLAYER_START_HP, ConstData.PLAYER_START_DMG));
            _mapManager.Init();
        }
        private void BindEvent()
        {
            _inputSystem.OnInputEvnet += OnInputHandle;
            
            _mapManager.SetEnemyEvent += _enemyManager.SetEnemy;
            
            _enemyManager.SetBoard(_mapManager);

            _player.FinishSlideEvent = FinishedPlayerSlide;
            _player.GetMoveContextFunc = _mapManager.GetMoveContext;

            _battleManager.SetCombatProvider(_enemyManager);
            _battleManager.SetPlayerCombatant(_player);
        }
        private void OnInputHandle(EDirectionType directionType)
        {
            if (_gameFlowType == EGameFlowType.None)
                _player.MoveRoute(directionType);
        }
        public void EnterGameFlow(EGameFlowType flowType)
        {
            _gameFlowType |= flowType;
        }
        public void ExitGameFlow(EGameFlowType flowType)
        {
            _gameFlowType &= ~flowType;
        }
        private void FinishedPlayerSlide()
        {
            _battleManager.StartBattleSequence();
        }
        public void ClearStage() { }
        public void GameFail()
        {
            Debug.Log("Á³´Ù!");
        }
        public void GameVictory() { }
    }
}
