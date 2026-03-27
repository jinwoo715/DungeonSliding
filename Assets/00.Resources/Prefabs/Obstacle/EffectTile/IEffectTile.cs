namespace JW.DungeonSliding
{
    public interface IEffectTile
    {
        public bool IsStepped { get; }
        public EEffectObjectType EffectType { get; }
        public MoveContext OnEnterTile(ref MoveContext moveContext);
        public void OnStepped();
    }
}