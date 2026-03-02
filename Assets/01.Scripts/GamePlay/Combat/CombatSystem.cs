using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class CombatSystem
    {
        private ICombatant _owner;
        private ICombatant _attackTarget;
        private ICombatant _lastAttacker;

        public CombatSystem(ICombatant combatant)
        {
            _owner = combatant;
        }

    }
}
