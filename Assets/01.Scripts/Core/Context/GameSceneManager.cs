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
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.GamePlay.Ability;

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

    public class GameSceneManager : MonoBehaviour, IGameResultService
    {
        private CombatEventBus _combatEventBus = new();
        private RewardManager _rewardManager = new ();

        private GameStateController _gameModeController;
        private IUIFader _uiFader;

        public event Action<GameResultPayload> OnGameWin;
        public event Action<GameResultPayload> OnGameLose;

        private IStageService _stageSerivce;
        private IPopupService _popupService;

        public IMoveable Moveable;
        public IStageViewer StageViewer;
        public IStatReadOnly PlayerStat;
        public List<AbilityDataBase> AbilityDataBases = new List<AbilityDataBase>();

        private float _gameTime;
        private bool _isGameFinished;

        public void Init(GameStateController gameModeController, IUIFader uiFader, IStageService stageSerivce, PlayerController playerController, IPopupService popupService)
        {
            GameManager.Sound.PlayBGM();

            _gameModeController = gameModeController;
            _uiFader = uiFader;
            _stageSerivce = stageSerivce;
            _popupService = popupService;

            playerController.OnPlayerDie += FailGame;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnClearStage, ClearFloor);

            _rewardManager.Init(_combatEventBus);
        }

        private void Update()
        {
            if (!_isGameFinished)
                _gameTime += Time.deltaTime;
        }

        public void AddedPlayerAbility(AbilityDataBase data)
        {
            AbilityDataBases.Add(data);
        }

        public void PrepareStage() 
        {
            StartCoroutine(CoWaitStartStage());
        }
        public void ClearFloor()
        {
            if (_isGameFinished)
                return;

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

            if (_isGameFinished)
                yield break;

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnEnterRoom);
        }

        public void FailGame()
        {
            CompleteGame(false);
        }
        public void VictoryGame()
        {
            CompleteGame(true);
        }

        private void CompleteGame(bool isVictory)
        {
            if (_isGameFinished)
                return;

            _isGameFinished = true;
            _gameModeController.EnterGameState(EGameStateType.GameOver);

            GameResultPayload payload = GetResultPayload();

            GameManager.Sound.StopBGM();

            if (isVictory)
            {
                OnGameWin?.Invoke(payload);
                GameManager.Sound.PlayEffectSound(EEffectSoundType.GameWin);
            }
            else
            {
                OnGameLose?.Invoke(payload);
                GameManager.Sound.PlayEffectSound(EEffectSoundType.GameDefeat);
            }

            ResetData();
        }

        public void RetryGame()
        {
            GameManager.Scene.LoadScene(SceneType.GameScene);
        }

        public void ReturnToLobby()
        {
            GameManager.Scene.LoadScene(SceneType.LobbyScene);
        }

        private void ResetData()
        {
            AbilityDataBases.Clear();
            _gameTime = 0;
            Moveable.SlidedCount = 0;
        }

        private GameResultPayload GetResultPayload()
        {
            GameResultPayload payload = new GameResultPayload();
            payload.GamePlay.TotalSlideCount = Moveable.SlidedCount;
            payload.GamePlay.TotalPlayTime = _gameTime;
            payload.GamePlay.CurrentFloor = StageViewer.CurrentFloor;
            payload.GamePlay.MaxFloor = StageViewer.MaxFloor;

            payload.PlayerInfoPayload.HP = PlayerStat.Get(ECreatureStatType.CurrentHP);
            payload.PlayerInfoPayload.MaxHP = PlayerStat.Get(ECreatureStatType.MaxHp);
            payload.PlayerInfoPayload.Damage = PlayerStat.Get(ECreatureStatType.Damage);
            payload.PlayerInfoPayload.Move = PlayerStat.Get(ECreatureStatType.CurrentMoveCount);
            payload.PlayerInfoPayload.MaxMove = PlayerStat.Get(ECreatureStatType.MaxMoveCount);
            payload.PlayerInfoPayload.Critical = PlayerStat.Get(ECreatureStatType.CriticalMultiplier);

            Debug.Log($"{payload.PlayerInfoPayload.HP}");

            payload.PlayerAbility.Lists = AbilityDataBases;

            return payload;
        }
    }
}
