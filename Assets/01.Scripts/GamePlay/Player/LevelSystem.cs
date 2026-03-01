using JW.Utility;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public class LevelSystem
    {
        public int Level { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int RequiredXp { get; private set; }

        public event Action<int> OnLevelUp;
        public event Action<int, int> OnChangedXp;

        public void Initialize(int level, int curXp)
        {
            Level = level;
            CurrentXp = curXp;
            RequiredXp = MathUtil.GetFib(Level + ConstData.LEVELUP_XP_OFFSET);
        }

        public void AddXp(int amount)
        {
            CurrentXp += amount;

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= amount;
                LevelUp();
            }

            OnChangedXp?.Invoke(CurrentXp, RequiredXp);
        }

        private void LevelUp()
        {
            Level++;
            RequiredXp = MathUtil.GetFib(Level + ConstData.LEVELUP_XP_OFFSET);

            OnLevelUp?.Invoke(Level);
        }
    }
}
