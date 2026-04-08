using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public interface IGameStateReader
    {
        public EGameStateType GameState { get; }
        public bool IsValidTurn();
    }
}
