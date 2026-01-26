namespace JW.DungeonSliding
{
    [System.Serializable]
    public struct EffectObjectData
    {
        public EEffectObjectType EffectObjectType;
        public Tile Point;

        //Teleport Tile¸¸ »ç¿ë
        public Tile TeleportPoint;

        public EffectObjectData(Tile point, EEffectObjectType type)
        {
            Point = point;
            EffectObjectType = type;
            TeleportPoint = Tile.Invalid;
        }
    }
}
