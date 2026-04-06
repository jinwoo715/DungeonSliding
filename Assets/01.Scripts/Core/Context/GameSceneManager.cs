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
    public class EventBus<T> where T : Enum
    {
        private Dictionary<T, Action> _events = new Dictionary<T, Action>();
        public void SubscribeEvent(T key, Action handler)
        {
            if(!_events.ContainsKey(key))
                _events[key] = handler;
            else
                _events[key] += handler;
        }
        public void UnSubscribe(T key, Action handler)
        {
            _events[key] -= handler;
        }
        public void Excute(T key)
        {
            _events[key].Invoke();
        }
        public void Clear()
        {
            _events.Clear();
        }
    }

    public class GameSceneManager : MonoBehaviour
    {
        private CombatEventBus _combatEventBus = new();
        private RewardManager _rewardManager = new ();

        private GameSequenceController _gameModeController;
        private IUIFader _uiFader;

        IStageService _stageSerivce;
        public void Init(GameSequenceController gameModeController, IUIFader uiFader, IStageService stageSerivce)
        {
            _gameModeController = gameModeController;
            _uiFader = uiFader;
            _stageSerivce = stageSerivce;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnClearStage, ClearFloor);

            _rewardManager.Init(_combatEventBus);
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

        public IEnumerator FailGame()
        {
            //TODO UI 팝업이 먼저 나와야하나?

            Debug.Log("졌다!");

            yield return _uiFader.FadeOut();

            SceneManager.LoadScene("LobbyScene");
        }

        public void OnPlayerDie() 
        { 

        }

        public void VictoryGame() { }
    }
}
