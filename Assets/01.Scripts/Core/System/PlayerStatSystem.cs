using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IPlayerStatProvider
    {
        public int Get(EPlayerStatType stat);
        public event Action<EPlayerStatType> OnStatChanged;
    }
    public interface IEnemyStatModifier
    {
        public int Get(EEnemyStatType stat);
        event Action<EEnemyStatType> OnStatChangedEvent;
    }
    public interface IEnemyStatUIService
    {
        public void Attach(Transform transform, IEnemyStatModifier enemyStatReadOnly);
        public void Detach(IEnemyStatModifier enemyStatReadOnly);
    }

}
