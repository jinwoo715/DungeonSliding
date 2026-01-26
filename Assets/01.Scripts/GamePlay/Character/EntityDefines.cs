using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public enum ECreatureStatus
    {
        None = 0,
        Bind = 1 << 0,
        Stun = 1 << 1
    }
}
