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
using System;
using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Stage;

namespace JW.DungeonSliding.GamePlay.Context
{
    public interface IActService
    {
        event Action<int, int> OnChangeActEvent;
        event Action<int, int> OnChangeFloorEvent;
    }

    public class GameSceneManager : MonoBehaviour, IActService
    {
        private RewardManager PlayerReward { get; set; }
        private MapManager _mapManager;
        private ICombatant _player;
        private EnemyManager _enemyManager;
        private BattleManager _battleManager;
        private InputSystem _inputSystem;
        private GameSequenceController _gameModeController;
        private IUIFader _uiFader;
        private IFieldObstacleService _obstacleRequest;

        public event Action<int, int> OnChangeActEvent;
        public event Action<int, int> OnChangeFloorEvent;

        public int Floor { get; private set; } = 0;
        public int Act { get; private set; } = 0;

        private int _floorPerAct;
        private int _actCount;
        private bool _isGameStart = false;

        IStageService _stageSerivce;
        public void Init(RewardManager reward, MapManager map,
            ICombatant player, EnemyManager enemyManager, BattleManager battleManager, 
            InputSystem input, GameSequenceController gameModeController, IUIFader uiFader
            , IFieldObstacleService obstacleRequest, IStageService stageSerivce)
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
            _stageSerivce = stageSerivce;

            _floorPerAct = GameManager.Config.Act.ActPerFloor;
            _actCount = GameManager.Config.Act.TotalFloor;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnClearStage, ClearFloor);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnStart, CheckGameOver);
        }

        public void PrepareStage() 
        {
            StartCoroutine(CoWaitStartStage());
        }

        private void UpdateActFloor()
        {
            //Floor++;

            //if (Floor > _floorPerAct-1)
            //{
            //    Act++;
            //    Floor -= _floorPerAct;

            //}

            //OnChangeActEvent?.Invoke(Act, _actCount);
            //OnChangeFloorEvent?.Invoke(Floor, _floorPerAct);
        }

        public void ClearFloor()
        {
            PrepareStage();
        }

        public IEnumerator CoWaitStartStage()
        {
            yield return null;

            yield return new WaitUntil(() => _gameModeController.GameMode == EGameModeType.Play);
            
            _gameModeController.EnterGameMode(EGameModeType.PrepareStage);

            yield return _uiFader.FadeOut();


            _stageSerivce.StartStage();
            //_obstacleRequest.ClearObstacles();

            //_mapManager.SetMap(Floor);

            UpdateActFloor();

            yield return _uiFader.FadeIn();

            _gameModeController.ExitGameMode(EGameModeType.PrepareStage);
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnEnterRoom);
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
