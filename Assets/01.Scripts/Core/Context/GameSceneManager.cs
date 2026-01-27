using UnityEngine;
using JW.Utility;
using System.Collections.Generic;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.Core.Flow;

namespace JW.DungeonSliding.GamePlay.Context
{
    public class GameSceneManager
    {
        public RewardManager PlayerReward { get; }
        public MapManager _mapManager;
        public Player _player;
        public EnemyManager _enemyManager;
        public BattleManager _battleManager;
        public InputSystem _inputSystem;
        public GameModeController _gameModeController;

        public int Floor { get; private set; }
        public RewardManager Reward => PlayerReward;

        public GameSceneManager(RewardManager reward, MapManager map, Player player, EnemyManager enemyManager, BattleManager battleManager, InputSystem input)
        {
            PlayerReward = reward;
            _mapManager = map;
            _player = player;
            _enemyManager = enemyManager;
            _battleManager = battleManager;
            _inputSystem = input;
        }
 
        public void StartStage()
        {
            _mapManager.SetMap(Floor);
        }
        public void ClearStage() 
        { 

        }

        public void FailGame()
        {
            Debug.Log("Á³´Ù!");
        }
        public void VictoryGame() { }
    }
}
