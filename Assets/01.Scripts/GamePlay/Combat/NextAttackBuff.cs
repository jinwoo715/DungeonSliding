namespace JW.DungeonSliding.GamePlay.Combat
{
    public struct NextAttackBuff
    {
        public int NextExtraAttackAcount;
        public int NextExtraDamage;
        public float NextExtraDamageMultiplier;

        public void AddDamage(int damage)
        {
            NextExtraDamage += damage;
        }
        public void AddExtraAttack(int count = 1)
        {
            NextExtraAttackAcount += count;
        }

        public void Reset() { NextExtraDamage = 0; NextExtraDamageMultiplier = 1.0f; NextExtraAttackAcount = 0; }
    }
}
