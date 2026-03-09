using JW.Utility;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public interface ILevelProgress
    {
        int CurrentLevel { get; }
        int CurrentXp { get; }
        int RequiredXp { get; } // 현재 레벨에서 다음 레벨로 가기 위한 총 경험치
        float ExpRatio => (float)CurrentXp / RequiredXp; // UI 바(Bar) 용도

        event Action OnChangedXp; // (level, currentExp, maxExp)
        event Action OnLevelUp; // (newLevel)

        void AddXp(int xp);
    }

    public class LevelSystem : ILevelProgress
    {
        public int CurrentLevel { get; private set; } = 1;
        public int CurrentXp { get; private set; }
        public int RequiredXp { get; private set; }

        public event Action OnLevelUp;
        public event Action OnChangedXp;

        public void Initialize()
        {
            CurrentLevel = 1;
            CurrentXp = 0;
            RequiredXp = MathUtil.GetFib(CurrentLevel + ConstData.LEVELUP_XP_OFFSET);
            OnChangedXp?.Invoke();
            OnLevelUp?.Invoke();
        }

        public void AddXp(int amount)
        {
            CurrentXp += amount;

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= amount;
                LevelUp();
            }

            OnChangedXp?.Invoke();
        }

        public void LevelUp()
        {
            CurrentLevel++;
            RequiredXp = MathUtil.GetFib(CurrentLevel + ConstData.LEVELUP_XP_OFFSET);

            OnLevelUp?.Invoke();
        }
    }
}
