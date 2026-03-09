using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public class GameModeController : IGameModeReader
    {
        private EGameModeType _gameFlowType = EGameModeType.Play;
        public bool IsCanMove => GameMode == 0;
        public EGameModeType GameMode => _gameFlowType;

        public Dictionary<EGameModeType, Action> GameModeEvent = new();

        public void Init()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnBattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnBattleEnd, ExitStartBattle);

            AbilityBusyCounter.OnWorkingAbility += () => EnterGameMode(EGameModeType.WorkingAbility);
            AbilityBusyCounter.OnWorkingAbility += () => ExitGameMode(EGameModeType.WorkingAbility);
        }

        public void SubscribeModeEvent(EGameModeType mode, Action action)
        {
            if (GameModeEvent.ContainsKey(mode))
            {
                GameModeEvent.Add(mode, delegate { });
            }

            GameModeEvent[mode] += action;
        }

        private void EnterSlideMode() => EnterGameMode(EGameModeType.Sliding);
        private void ExitSlideMode() => ExitGameMode(EGameModeType.Sliding);
        private void EnterStartBattle() => EnterGameMode(EGameModeType.Battle);
        private void ExitStartBattle() => ExitGameMode(EGameModeType.Battle);
        private void EnterAbilityUI() => EnterGameMode(EGameModeType.AbilityUI);
        private void ExitAbilityUI() => ExitGameMode(EGameModeType.AbilityUI);

        public void EnterGameMode(EGameModeType flowType)
        {
            _gameFlowType |= flowType;
        }
        public void ExitGameMode(EGameModeType flowType)
        {
            _gameFlowType &= ~flowType;
        }
        public void Clear()
        {
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameEventTrigger.OnBattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameEventTrigger.OnBattleEnd, ExitStartBattle);

            AbilityBusyCounter.OnWorkingAbility -= () => EnterGameMode(EGameModeType.WorkingAbility);
            AbilityBusyCounter.OnWorkingAbility -= () => ExitGameMode(EGameModeType.WorkingAbility);
        }
    }
}
