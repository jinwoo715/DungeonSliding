namespace JW.SlidingPuzzle
{
    public interface IEffectObject
    {
        public EEffectObjectType EffectType { get; }
        public MoveContext OnEnterTile(ref MoveContext moveContext);
    }
}