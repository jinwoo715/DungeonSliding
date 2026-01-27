using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{


    public class GameModeController : IGameModeChanger
    {
        private EGameModeType _gameFlowType = EGameModeType.None;

        public EGameModeType Flow => _gameFlowType;

        public bool IsCanMove => Flow == 0;

        public void EnterGameMode(EGameModeType flowType)
        {
            _gameFlowType |= flowType;
        }
        public void ExitGameMode(EGameModeType flowType)
        {
            _gameFlowType &= ~flowType;
        }
    }
}
