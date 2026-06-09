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

    public interface IPopupService
    {
        void ShowOneButtonPopup(string name, string desc, ButtonSet buttonSet);
    }

    public struct ButtonSet
    {
        public string ButtonName;
        public Action ButtonEvent;
    }

    public class GameSceneManager : MonoBehaviour
    {
        private CombatEventBus _combatEventBus = new();
        private RewardManager _rewardManager = new ();

        private GameStateController _gameModeController;
        private IUIFader _uiFader;

        public event Action OnFailGame;
        public event Action OnVictoryGame;

        IStageService _stageSerivce;
        IPopupService _popupService;

        public void Init(GameStateController gameModeController, IUIFader uiFader, IStageService stageSerivce, PlayerController playerController, IPopupService popupService)
        {
            _gameModeController = gameModeController;
            _uiFader = uiFader;
            _stageSerivce = stageSerivce;
            _popupService = popupService;

            playerController.OnPlayerDie += FailGame;

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

            yield return new WaitUntil(() => _gameModeController.GameState == EGameStateType.Play);
            
            _gameModeController.EnterGameState(EGameStateType.PrepareStage);

            yield return _uiFader.FadeOut();

            _stageSerivce.StartStage();

            yield return _uiFader.FadeIn();

            _gameModeController.ExitGameState(EGameStateType.PrepareStage);
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnEnterRoom);
        }

        public void FailGame()
        {
            ButtonSet buttonSet = new ButtonSet();
            buttonSet.ButtonName = "로비";
            buttonSet.ButtonEvent = () => { GameManager.Scene.LoadScene(SceneType.LobbyScene); };
            _popupService.ShowOneButtonPopup("패배", "게임에 패배하였습니다.", buttonSet);
            OnFailGame?.Invoke();
        }
        public void VictoryGame() 
        {
            ButtonSet buttonSet = new ButtonSet();
            buttonSet.ButtonName = "로비";
            buttonSet.ButtonEvent = () => { GameManager.Scene.LoadScene(SceneType.LobbyScene); };
            _popupService.ShowOneButtonPopup("승리", "게임에 승리하였습니다.", buttonSet);
            OnVictoryGame?.Invoke();
        }
    }
}
