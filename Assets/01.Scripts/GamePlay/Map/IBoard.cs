using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.Map
{
    public interface IBoard
    {
        public void RegisterTileBoard(Tile point, bool isWalkable);
        public void RegisterEffectObject(Tile point, IEffectTile effectObj);
        public void UnRegisterEffectObject(Tile point);
        public void RegisterEnemyTile(Tile point);
        public void UnRegisterEnemyTile(Tile point);
        public void RegisterObstacleTile(Tile point);
        public void UnRegisterObstacleTile(Tile point);
        public void ClearEnemyBoard();
    }

    public interface IBoardCreatureRegister
    {

    }

    public interface ITileCheckService
    {
        public bool IsRouteTile(Tile point);
    }

    public interface IMoveContextProvider
    {
        public MoveContext GetMoveContext(Tile startPoint, EDirectionType direction, ETileEnterType enterType);
    }
}
