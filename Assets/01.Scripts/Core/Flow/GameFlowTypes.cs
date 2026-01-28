using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public enum EGameModeType
    {
        Play = 0,
        PrepareStage = 1 << 5,
        SettingEnemy = 1 << 1,
        Sliding = 1 << 2,
        Battle = 1 << 3,
        AbilityUI = 1 << 4,
    }
}
