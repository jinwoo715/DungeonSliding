using JW.DungeonSliding.GamePlay;
using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public class GameModeController : IGameModeChanger
    {
        private EGameModeType _gameFlowType = EGameModeType.Play;
        public EGameModeType Flow => _gameFlowType;
        public bool IsCanMove => Flow == 0;

        public void Init()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.SlideStart, EnterSlideMode);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.SlideEnd, ExitSlideMode);

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.BattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.BattleEnd, ExitStartBattle);
        }
        private void EnterSlideMode() => EnterGameMode(EGameModeType.Sliding);
        private void ExitSlideMode() => ExitGameMode(EGameModeType.Sliding);

        private void EnterStartBattle() => EnterGameMode(EGameModeType.Battle);
        private void ExitStartBattle() => ExitGameMode(EGameModeType.Battle);

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
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.SlideStart, EnterSlideMode);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.SlideEnd, ExitSlideMode);

            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.BattleStart, EnterStartBattle);
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameTriggerType.BattleEnd, ExitStartBattle);
        }
    }
}
