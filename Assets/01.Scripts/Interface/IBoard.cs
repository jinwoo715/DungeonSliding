using System.Collections.Generic;

namespace JW.SlidingPuzzle
{
    public interface IBoard
    {
        public void RegisterTileBoard(TilePoint point, bool isWalkable);

        public void RegisterEffectObject(TilePoint point, IEffectObject effectObj);
        public void UnRegisterEffectObject(TilePoint point);
        public void RegisterEnemyBoard(TilePoint point, ICombatant combatant);
        public void UnRegisterEnemyBoard(TilePoint point);
        public void ClearEnemyBoard();
    }


    public interface ICombatProvider
    {
        public bool TryGetCombatant(TilePoint tilePoint, out ICombatant combatant);
        public List<ICombatant> GetAllCombatant();
    }
}
