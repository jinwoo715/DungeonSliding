namespace JW.DungeonSliding
{
    public struct MoveContext
    {
        public EDirectionType Direction;
        public ESlideResultType ResultType;
        public Tile DestTile;
        public ETileEnterType EnterType;

        public int Damage;

        public MoveContext(Tile point, EDirectionType direction, ETileEnterType enterType)
        {
            Damage = 0;
            DestTile = point;
            Direction = direction;
            ResultType = ESlideResultType.Move;
            EnterType = enterType;
        }
    }
}