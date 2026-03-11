using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IStatModifier
    {
        public event Action<ECreatureStatType> OnStatChanged;
        public void ModifyStat(StatModifierContext modifierContext);
    }
}
