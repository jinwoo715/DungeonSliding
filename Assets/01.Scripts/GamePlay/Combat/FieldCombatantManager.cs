using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class FieldCombatantManager : ICombatantSensor
    {
        private ICombatant _playerCombatant;
        private ICombatProvider _enemyCombatProvider;

        public FieldCombatantManager(ICombatProvider combatProvider, ICombatant player)
        {
            _enemyCombatProvider = combatProvider;
            _playerCombatant = player;
        }

        public ICombatant PlayerCombatant { get => _playerCombatant;}
        public List<ICombatant> AllEnemyCombatants => _enemyCombatProvider.GetAllActiveCombatant();
        public bool GetCombatant(Tile tile, ECretureType targetType, out ICombatant combatant)
        {
            switch (targetType)
            {
                case ECretureType.Player:
                    if (_playerCombatant.TilePosition == tile)
                    {
                        combatant = _playerCombatant;
                        return true;
                    }
                    break;
                case ECretureType.Enemy:
                    if (_enemyCombatProvider.TryGetCombatant(tile, out ICombatant combat))
                    {
                        combatant = combat;
                        return true;
                    }
                    break;
            }

            combatant = default;
            return false;
        }
        public int GetNearEnemyCount(Tile pivot)
        {
            int[,] positions = new int[8,2] 
            { 
                {-1, 1 }, { 0, 1 } ,{ 1, 1 },
                {-1, 0 },           { 1, 0 },
                {-1,-1 }, { 0,-1 } ,{ 1,-1 }
            };

            int count = 0;

            for (int i = 0; i < 8; i++)
            {
                Tile search = new Tile(pivot.XPos + positions[i, 0], pivot.ZPos + positions[i, 1]);

                if (_enemyCombatProvider.TryGetCombatant(search, out ICombatant combat)) count++;
            }

            return count;
        }
    }
}
