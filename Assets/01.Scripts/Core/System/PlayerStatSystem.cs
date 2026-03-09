using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IStatReadOnly
    {
        public int Get(ECreatureStatType stat);
    }
    public interface IStatModifier
    {
        public event Action<ECreatureStatType> OnStatChanged;
        public void ModifyStat(StatModifierContext modifierContext);
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
