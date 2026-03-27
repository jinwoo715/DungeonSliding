using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Move;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public class GameSequenceController : IGameModeReader
    {
        private EGameModeType _gameFlowType = EGameModeType.Play;
        public bool IsCanMove => GameMode == 0;
        public EGameModeType GameMode => _gameFlowType;

        public Dictionary<EGameModeType, Action> GameModeEvent = new();

        IRouteService _routeService;
        IBattleResult _battleResult;
        public bool IsValidTurn()
        {
            bool result = (_battleResult.IsBattleTurn() || _routeService.LastMoveTileCount != 0);
            return result;
        }

        public void Init(IRouteService routeService, IBattleResult battleResult)
        {
            _routeService = routeService;
            _battleResult = battleResult;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnBattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnBattleEnd, ExitStartBattle);

            AbilityBusyCounter.OnWorkingAbility += () => EnterGameMode(EGameModeType.WorkingAbility);
            AbilityBusyCounter.OnEndAllAbility += () => ExitGameMode(EGameModeType.WorkingAbility);
        }
        public void SubscribeModeEvent(EGameModeType mode, Action action)
        {
            if (GameModeEvent.ContainsKey(mode))
            {
                GameModeEvent.Add(mode, delegate { });
            }

            GameModeEvent[mode] += action;
        }
        private void EnterStartBattle() => EnterGameMode(EGameModeType.Battle);
        private void ExitStartBattle() 
        {
            ExitGameMode(EGameModeType.Battle);

            if (IsValidTurn())
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnEnd);
            }
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnStart);
        }
        public void EnterGameMode(EGameModeType flowType)
        {
            if (flowType == EGameModeType.WorkingAbility) Debug.Log("Enter Work");
            _gameFlowType |= flowType;
        }
        public void ExitGameMode(EGameModeType flowType)
        {
            if (flowType == EGameModeType.WorkingAbility) Debug.Log("Exit Work");
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
