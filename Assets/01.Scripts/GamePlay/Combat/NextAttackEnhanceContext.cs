using System;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface INextAttackEnhancer
    {
        public event Action<int> OnChangedNextAttackDamage;
        public event Action<int> OnChangedNextAttackCount;

        int FixedAddDamage { get; }
        float MultipleAddDamage { get; }
        int FinalExtraAttackCount { get; }

        int FinalEnhanceDamage { get; }

        void AddNextAttackDamage(int damage);
        void AddNextAttackDamageMulti(float multi);
        void AddNextAttackCount(int count);
        void Clear();
    }

    public interface IReadOnlyNextAttackEnhancer
    {
        public int FixedAddDamage { get;}
        public float MultipleAddDamage { get; }
        public int ExtraAttackCount { get; }
    }

    public class NextAttackEnhanceContext : IReadOnlyNextAttackEnhancer
    {
        public int NextExtraAttackAcount = 0;
        public int NextExtraDamage = 0;
        public float NextExtraDamageMultiplier = 1;

        public int FixedAddDamage => throw new NotImplementedException();
        public float MultipleAddDamage => throw new NotImplementedException();
        public int ExtraAttackCount => throw new NotImplementedException();

        public event Action<int> OnChangedNextAttackDamage;
        public event Action<int> OnChangedNextAttackCount;

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

        public void AddNextAttackCount(int count)
        {
            throw new NotImplementedException();
        }

        public void AddNextAttackDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public void AddNextAttackDamageMulti(float multi)
        {
            throw new NotImplementedException();
        }

        public void Reset() { NextExtraDamage = 0; NextExtraDamageMultiplier = 1.0f; NextExtraAttackAcount = 0; }
    }
}
