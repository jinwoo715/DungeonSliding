using JW.DungeonSliding.Core;
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

        event Action<int, int> OnChangedXp; // (currentEp, maxEp)
        event Action<int> OnLevelUp; // (newLevel)

        void AddXp(int xp);
    }

    public class LevelSystem : ILevelProgress
    {
        public int CurrentLevel { get; private set; }
        public int CurrentXp { get; private set; }
        public int RequiredXp { get; private set; }

        public event Action<int> OnLevelUp;
        public event Action<int, int> OnChangedXp;

        public void Initialize()
        {
            CurrentLevel = 1;
            CurrentXp = 0;
            RequiredXp = MathUtil.GetFib(CurrentLevel + ConstData.LEVELUP_XP_OFFSET);

            OnChangedXp?.Invoke(CurrentXp, RequiredXp);
        }

        public void AddXp(int amount)
        {
            CurrentXp += amount;

            while (CurrentXp >= RequiredXp)
            {
                CurrentXp -= RequiredXp;
                LevelUp();
            }

            OnChangedXp?.Invoke(CurrentXp, RequiredXp);
        }

        public void LevelUp()
        {
            CurrentLevel++;
            RequiredXp = MathUtil.GetFib(CurrentLevel + ConstData.LEVELUP_XP_OFFSET);

            OnLevelUp?.Invoke(CurrentLevel);

            GameManager.Sound.PlayEffectSound(EEffectSoundType.LevelUp);
            Debug.Log("On Level Up");
        }
    }
}
