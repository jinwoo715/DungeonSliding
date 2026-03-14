using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public enum ECreatureStatus
    {
        None = 0,
        Bind = 1 << 0,
        Stun = 1 << 1,
        Knockback = 1 << 2,
        Barrier = 1 << 3,
        Hide = 1 << 4,
    }
}
