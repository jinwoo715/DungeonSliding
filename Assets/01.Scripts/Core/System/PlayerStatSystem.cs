using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IPlayerStatReadOnly
    {
        public int Get(EPlayerStat stat);
        public Action<EPlayerStat> OnStatChanged { get; set; }
    }
    public interface IEnemyStatReadOnly
    {
        public int Get(EEnemyStatType stat);
        event Action<EEnemyStatType> OnStatChanged;
    }
    public interface IEnemyStatUIService
    {
        public void Attach(Transform transform, IEnemyStatReadOnly enemyStatReadOnly);
        public void Detach(IEnemyStatReadOnly enemyStatReadOnly);
    }
 
}
