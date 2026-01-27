using UnityEngine;

namespace JW.DungeonSliding.Core.Flow
{
    public enum EGameModeType
    {
        None = 0,
        MapLoading = 1 << 0,
        SettingEnemy = 1 << 1,
        Sliding = 1 << 2,
        Battle = 1 << 3,
        AbilityUI = 1 << 4,
    }
}
