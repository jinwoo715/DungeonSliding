using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public interface IGameModeChanger
    {
        public void EnterGameMode(EGameModeType flowType);
        public void ExitGameMode(EGameModeType flowType);
    }
}
