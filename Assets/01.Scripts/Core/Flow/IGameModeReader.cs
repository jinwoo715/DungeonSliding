using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public interface IGameModeReader
    {
        public EGameModeType GameMode { get; }
    }
}
