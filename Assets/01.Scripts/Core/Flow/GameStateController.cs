using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Move;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public interface IGameStateModifier
    {
        void EnterGameState(EGameStateType flowType);
        void ExitGameState(EGameStateType flowType);

        event Action<bool> OnChangeMoveState;
        event Action<EGameStateType> OnExitState;
    }

    public class GameStateController : IGameStateReader, IGameStateModifier
    {
        public EGameStateType GameState => _gameFlowType;
        private EGameStateType _gameFlowType = EGameStateType.Play;

        public event Action<bool> OnChangeMoveState;
        public event Action<EGameStateType> OnExitState;

        IRouteService _routeService;
        IBattleResult _battleResult;

        public void Init(IRouteService routeService, IBattleResult battleResult)
        {
            _routeService = routeService;
            _battleResult = battleResult;

            OnExitState += TurnEndProcess;

            AbilityBusyCounter.OnWorkingAbility += () => EnterGameState(EGameStateType.WorkingAbility);
            AbilityBusyCounter.OnEndAllAbility += () => ExitGameState(EGameStateType.WorkingAbility);
        }
        public bool IsValidTurn()
        {
            bool result = (_battleResult.IsBattleTurn() || _routeService.LastMoveTileCount != 0);
            return result;
        }
        private void TurnEndProcess(EGameStateType stateType) 
        {
            if (IsValidTurn())
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnEnd);
            }
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnStart);
        }
        public void EnterGameState(EGameStateType flowType)
        {
            if (flowType == EGameStateType.WorkingAbility) Debug.Log("Enter Work");
            
            _gameFlowType |= flowType;

            OnChangeMoveState?.Invoke(_gameFlowType == 0);
        }
        public void ExitGameState(EGameStateType flowType)
        {
            if (flowType == EGameStateType.WorkingAbility) Debug.Log("Exit Work");
            _gameFlowType &= ~flowType;

            OnChangeMoveState?.Invoke(_gameFlowType == 0);
            OnExitState?.Invoke(flowType);
        }
        public void Clear()
        {
            AbilityBusyCounter.OnWorkingAbility -= () => EnterGameState(EGameStateType.WorkingAbility);
            AbilityBusyCounter.OnWorkingAbility -= () => ExitGameState(EGameStateType.WorkingAbility);
        }
    }
}
