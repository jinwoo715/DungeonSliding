using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public enum EGameStateType
    {
        Play = 0,
        PrepareStage = 1 << 1,
        WorkingAbility = 1 << 2,
        SettingEnemy = 1 << 3,
        Sliding = 1 << 4,
        Battle = 1 << 5,
        AbilityUI = 1 << 6,
        GameOver = 1 << 7,
    }
}
