using UnityEngine;
using JW.Utility;
using System.Collections.Generic;

namespace JW.SlidingPuzzle
{
    public class GameSceneManager : MonoBehaviour
    {
        private static GameSceneManager _instance;
        public static GameSceneManager Instance => _instance;

        private PlayerRewardManager _playerRewardManager = new PlayerRewardManager();

        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Player _player;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;

        [SerializeField] private List<MapData> _mapDatas;
        private ShuffleBag<MapData> _mapBag;

        private EGameFlowType _gameFlowType = EGameFlowType.None;

        public PlayerRewardManager Reward => _playerRewardManager;

        public int Floor { get; private set; }

        private void Awake()
        {
            if (_instance != null)
                Destroy(_instance.gameObject);

            _instance = this;
        }
        private void OnDestroy()
        {
            _instance = null;
        }
        private void Start()
        {
            _mapBag = new ShuffleBag<MapData>(_mapDatas);

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
