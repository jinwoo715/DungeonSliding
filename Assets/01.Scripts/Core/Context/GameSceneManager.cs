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
        private ICombatant _player;
        private GameSequenceController _gameModeController;
        private IUIFader _uiFader;

        public event Action<int, int> OnChangeActEvent;
        public event Action<int, int> OnChangeFloorEvent;

        IStageService _stageSerivce;
        public void Init(ICombatant player, GameSequenceController gameModeController, IUIFader uiFader, IStageService stageSerivce)
        {
            _player = player;

            _gameModeController = gameModeController;
            _uiFader = uiFader;
            _stageSerivce = stageSerivce;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnClearStage, ClearFloor);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnStart, CheckGameOver);
        }

        public void PrepareStage() 
        {
            StartCoroutine(CoWaitStartStage());
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
