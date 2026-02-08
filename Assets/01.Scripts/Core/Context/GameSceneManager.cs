using UnityEngine;
using JW.Utility;
using System.Collections.Generic;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.Core.Flow;
using System.Collections;
using JW.DungeonSliding.UI;
using UnityEngine.SceneManagement;

namespace JW.DungeonSliding.GamePlay.Context
{
    public class GameSceneManager : MonoBehaviour
    {
        private RewardManager PlayerReward { get; set; }
        private MapManager _mapManager;
        private Player _player;
        private EnemyManager _enemyManager;
        private BattleManager _battleManager;
        private InputSystem _inputSystem;
        private GameModeController _gameModeController;
        private IUIFader _uiFader;
        private IObstacleRequest _obstacleRequest;
        
        public int Floor { get; private set; }
        public void Init(RewardManager reward, MapManager map, 
            Player player, EnemyManager enemyManager, BattleManager battleManager, 
            InputSystem input, GameModeController gameModeController, IUIFader uiFader
            , IObstacleRequest obstacleRequest)
        {
            PlayerReward = reward;
            _mapManager = map;
            _player = player;
            _enemyManager = enemyManager;
            _battleManager = battleManager;
            _inputSystem = input;
            _gameModeController = gameModeController;
            _uiFader = uiFader;
            _obstacleRequest = obstacleRequest;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnClearStage, PrepareStage);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, CheckGameOver);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnLevelUp);
            }
        }

        public void PrepareStage() 
        {
            StartCoroutine(CoWaitStartStage());
        }
        public IEnumerator CoWaitStartStage()
        {
            yield return new WaitUntil(() => _gameModeController.Flow == EGameModeType.Play);
            
            _gameModeController.EnterGameMode(EGameModeType.PrepareStage);

            yield return _uiFader.FadeOut();

            _obstacleRequest.ClearObstacles();

            _mapManager.SetMap(Floor);

            yield return _uiFader.FadeIn();

            _gameModeController.ExitGameMode(EGameModeType.PrepareStage);
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnEnterRoom);
        }

        public void CheckGameOver()
        {
            if (!_player.IsActive)
                StartCoroutine(FailGame());
        }
        public IEnumerator FailGame()
        {
            //TODO UI 팝업이 먼저 나와야하나?

            Debug.Log("졌다!");

            yield return _uiFader.FadeOut();

            SceneManager.LoadScene("LobbyScene");
        }
        public void VictoryGame() { }
    }
}
