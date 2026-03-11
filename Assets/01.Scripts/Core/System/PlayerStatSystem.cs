using JW.DungeonSliding.GamePlay.Combat;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public interface IEnemyStatUIService
    {
        public void Attach(Transform transform, ICombatant combatant);
        public void Detach(ICombatant combatant);
        public void HideAll();
        public void ShowAll();
    }

}
