namespace JW.DungeonSliding
{
    public interface IEffectObject
    {
        public EEffectObjectType EffectType { get; }
        public MoveContext OnEnterTile(ref MoveContext moveContext);
    }
}