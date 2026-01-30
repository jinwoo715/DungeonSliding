using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IStatReadOnly
    {
        public float Get(EPlayerStat stat);
        public float GetMax(EPlayerStat stat); // HP면 MaxHp 같은 매핑 처리
        public event Action<EPlayerStat> OnStatChanged;
    }

    public interface IStatModifier
    {
        void ModifyStat(ApplyStatContext ctx);
    }

    public class PlayerStatSystem : IStatReadOnly, IStatModifier
    {
        public event Action<EPlayerStat> OnStatChanged;

        public float Get(EPlayerStat stat)
        {
            throw new NotImplementedException();
        }

        public float GetMax(EPlayerStat stat)
        {
            throw new NotImplementedException();
        }

        public void ModifyStat(ApplyStatContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
