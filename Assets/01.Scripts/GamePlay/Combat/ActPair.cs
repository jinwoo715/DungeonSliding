namespace JW.DungeonSliding.GamePlay.Combat
{
    public struct ActPair
    {
        public ICombatant Attacker;
        public ICombatant Target;
        public ActPair(ICombatant attacker, ICombatant target)
        {
            Attacker = attacker;
            Target = target;
        }
    }
}
