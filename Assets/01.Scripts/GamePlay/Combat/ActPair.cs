namespace JW.DungeonSliding.GamePlay.Combat
{
    public enum EAttackType
    {
        Nomal,
        Counter
    }
    public struct ActPair
    {
        public ICombatant Attacker;
        public ICombatant Target;
        public EAttackType AttackType;

        public ActPair(ICombatant attacker, ICombatant target, EAttackType attackType)
        {
            Attacker = attacker;
            Target = target;
            AttackType = attackType;
        }
    }
}
