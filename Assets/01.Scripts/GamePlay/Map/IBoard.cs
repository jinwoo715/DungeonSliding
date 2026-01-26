using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.Map
{
    public interface IBoard
    {
        public void RegisterTileBoard(Tile point, bool isWalkable);

        public void RegisterEffectObject(Tile point, IEffectObject effectObj);
        public void UnRegisterEffectObject(Tile point);
        public void RegisterEnemyBoard(Tile point, ICombatant combatant);
        public void UnRegisterEnemyBoard(Tile point);
        public void ClearEnemyBoard();
    }
}
