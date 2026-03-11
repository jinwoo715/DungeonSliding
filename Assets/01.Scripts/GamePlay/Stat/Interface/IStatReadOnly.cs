using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IStatReadOnly
    {
        public int Get(ECreatureStatType stat);
    }
}
