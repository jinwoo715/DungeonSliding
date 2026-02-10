using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IPlayerStatReader
    {
        public int Get(EPlayerStatType stat);
        public event Action<EPlayerStatType> OnStatChanged;
    }
    public interface IEnemyStatModifier
    {
        int Get(EEnemyStatType stat);
        event Action<EEnemyStatType> OnStatChangedEvent;
        void SetEnemyStat(EEnemyStatType stat, int value);
        void ModifyEnemyStat(EnemyApplyStatContext context);
    }
    public interface IEnemyStatUIService
    {
        public void Attach(Transform transform, IEnemyStatModifier enemyStatReadOnly);
        public void Detach(IEnemyStatModifier enemyStatReadOnly);
        public void HideAll();
        public void ShowAll();
    }

}
