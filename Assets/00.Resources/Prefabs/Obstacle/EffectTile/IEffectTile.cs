namespace JW.DungeonSliding
{
    public interface IEffectTile
    {
        public EEffectObjectType EffectType { get; }
        public MoveContext OnEnterTile(ref MoveContext moveContext);
    }
}