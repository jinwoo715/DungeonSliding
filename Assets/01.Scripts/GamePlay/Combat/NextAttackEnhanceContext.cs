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
        bool IsGuaranteedCritical { get; }

        int FinalEnhanceDamage { get; }

        void AddNextAttackDamage(int damage);
        void AddNextAttackDamageMulti(float multi);
        void AddNextAttackCount(int count);
        void GuaranteedCritical();
        void Clear();
    }

    public interface IReadOnlyNextAttackEnhancer
    {
        public int FixedAddDamage { get;}
        public float MultipleAddDamage { get; }
        public int ExtraAttackCount { get; }
    }

   
}
