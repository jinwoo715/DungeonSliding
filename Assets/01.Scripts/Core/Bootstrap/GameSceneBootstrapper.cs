using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Bootstrap 
{
    public class GameSceneBootstrapper : MonoBehaviour
    {
        private GameSceneContext _gameSceneContext;

        private RewardManager _rewardManager = new RewardManager();
        [SerializeField] private MapManager _mapManager;
        [SerializeField] private Player _player;
        [SerializeField] private EnemyManager _enemyManager;
        [SerializeField] private BattleManager _battleManager;
        [SerializeField] private InputSystem _inputSystem;

        private void Awake()
        {
            _gameSceneContext = new GameSceneContext(_rewardManager, _mapManager, _player, _enemyManager, _battleManager, _inputSystem);

        }
    }
}