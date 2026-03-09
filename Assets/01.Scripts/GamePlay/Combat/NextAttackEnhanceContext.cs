namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface INextAttackEnhancer
    {
        void AddNextAttackDamage(int damage);
        void AddNextAttackDamageMulti(float multi);
        void AddNextAttackCount(int count);
    }

    public class NextAttackEnhanceContext
    {
        public int NextExtraAttackAcount = 0;
        public int NextExtraDamage = 0;
        public float NextExtraDamageMultiplier = 1;

        public void AddDamage(int damage)
        {
            NextExtraDamage += damage;
        }
        public void AddDamageMulti(float multi)
        {
            NextExtraDamageMultiplier += multi;
        }
        public void AddExtraAttack(int count = 1)
        {
            NextExtraAttackAcount += count;
        }

        public void Reset() { NextExtraDamage = 0; NextExtraDamageMultiplier = 1.0f; NextExtraAttackAcount = 0; }
    }
}
