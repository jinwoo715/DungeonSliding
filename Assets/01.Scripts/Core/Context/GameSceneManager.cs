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

namespace JW.DungeonSliding.GamePlay.Context
{
    public class GameSceneManager : MonoBehaviour
    {
        public RewardManager PlayerReward { get; private set; }
        public MapManager _mapManager;
        public Player _player;
        public EnemyManager _enemyManager;
        public BattleManager _battleManager;
        public InputSystem _inputSystem;
        public GameModeController _gameModeController;
        public IUIFader _uiFader;

        public int Floor { get; private set; }
        public RewardManager Reward => PlayerReward;
        public void Init(RewardManager reward, MapManager map, 
            Player player, EnemyManager enemyManager, BattleManager battleManager, 
            InputSystem input, GameModeController gameModeController, IUIFader uiFader)
        {
            PlayerReward = reward;
            _mapManager = map;
            _player = player;
            _enemyManager = enemyManager;
            _battleManager = battleManager;
            _inputSystem = input;
            _gameModeController = gameModeController;
            _uiFader = uiFader;
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.ClearStage, PrepareStage);
        }

        public void PrepareStage() 
        {
            _gameModeController.EnterGameMode(EGameModeType.PrepareStage);

            StartCoroutine(CoWaitStartStage());
        }

        public IEnumerator CoWaitStartStage()
        {
            bool isFadeOutDone = false;

            //GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.FadeOutFin, () => { isFadeOutDone = true; });
            //GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.FadeOutStart);

            yield return _uiFader.FadeOut();

            //yield return new WaitUntil(() => isFadeOutDone == true);

            //GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.FadeOutFin, () => { isFadeOutDone = true; });

            _mapManager.SetMap(Floor);

            yield return _uiFader.FadeIn();
            //bool isFadeInDone = false;

            //GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.FadeInFin, () => { isFadeInDone = true; });
            //GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.FadeInStart);

            //yield return new WaitUntil(() => isFadeInDone == true);

            //GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.FadeInFin, () => { isFadeInDone = true; });

            _gameModeController.ExitGameMode(EGameModeType.PrepareStage);
        }

        public void FailGame()
        {
            Debug.Log("Á³´Ù!");
        }
        public void VictoryGame() { }
    }
}
