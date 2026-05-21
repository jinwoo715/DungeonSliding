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

        private Action OnEnterWorkingAbility;
        private Action OnExitWorkingAbility;
        public void Init(IRouteService routeService, IBattleResult battleResult)
        {
            _routeService = routeService;
            _battleResult = battleResult;

            OnExitState += TurnEndProcess;

            OnEnterWorkingAbility += () => EnterGameState(EGameStateType.WorkingAbility);
            OnExitWorkingAbility += () => ExitGameState(EGameStateType.WorkingAbility);

            AbilityBusyCounter.OnWorkingAbility += OnEnterWorkingAbility;
            AbilityBusyCounter.OnEndAllAbility += OnExitWorkingAbility;
        }
        public bool IsValidTurn()
        {
            bool result = (_battleResult.IsBattleTurn() || _routeService.LastMoveTileCount != 0);
            return result;
        }
        private void TurnEndProcess(EGameStateType stateType) 
        {
            if (stateType == EGameStateType.Battle)
            {
                if (IsValidTurn())
                {
                    GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnEnd);
                }
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnStart);
            }
        }

        public void EnterWorkingAbility()
        {
            EnterGameState(EGameStateType.WorkingAbility);
        }
        public void ExitWorkingAbility()
        {
            ExitGameState(EGameStateType.WorkingAbility);
        }

        public void EnterGameState(EGameStateType flowType)
        {
            _gameFlowType |= flowType;

            Debug.Log($"Enter : {flowType}");

            OnChangeMoveState?.Invoke(_gameFlowType == 0);
        }
        public void ExitGameState(EGameStateType flowType)
        {
            _gameFlowType &= ~flowType;

            Debug.Log($"Exit : {flowType}");

            OnChangeMoveState?.Invoke(_gameFlowType == 0);
            OnExitState?.Invoke(flowType);
        }
        public void Clear()
        {
            AbilityBusyCounter.OnWorkingAbility -= OnEnterWorkingAbility;
            AbilityBusyCounter.OnEndAllAbility -= OnExitWorkingAbility;

            OnEnterWorkingAbility = null;
            OnExitWorkingAbility = null;
        }
    }
}
