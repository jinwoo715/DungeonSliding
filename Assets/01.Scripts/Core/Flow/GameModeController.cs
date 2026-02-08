using JW.DungeonSliding.GamePlay;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public class GameModeController : IGameModeReader
    {
        private EGameModeType _gameFlowType = EGameModeType.Play;
        public EGameModeType Flow => _gameFlowType;
        public bool IsCanMove => Flow == 0;
        public EGameModeType GameMode => _gameFlowType;

        public Dictionary<EGameModeType, Action> GameModeEvent = new();

        public void Init()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnSlideStart, EnterSlideMode);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnSlideEnd, ExitSlideMode);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnMoveEnd, ExitSlideMode);

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnBattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnBattleEnd, ExitStartBattle);

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnShowAbility, EnterAbilityUI);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnHideAbility, ExitAbilityUI);
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
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnSlideStart, EnterSlideMode);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnSlideEnd, ExitSlideMode);

            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnBattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnBattleEnd, ExitStartBattle);

            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnShowAbility, EnterAbilityUI);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.OnHideAbility, ExitAbilityUI);
        }
    }
}
