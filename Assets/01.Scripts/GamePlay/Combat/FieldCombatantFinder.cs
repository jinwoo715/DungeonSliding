using JW.DungeonSliding.GamePlay.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class FieldCombatantFinder : ICombatantSensor
    {
        private ICombatant _playerCombatant;
        private ICombatProvider _enemyCombatProvider;

        public ICombatant PlayerCombatant { get => _playerCombatant;}
        public List<ICombatant> AllEnemyCombatants => _enemyCombatProvider.GetAllActiveCombatant();
        public void Init(ICombatProvider combatProvider, ICombatant player)
        {
            _enemyCombatProvider = combatProvider;
            _playerCombatant = player;
        }
        public bool TryGetCombatant(Tile tile, ECreatureType targetType, out ICombatant combatant)
        {
            combatant = default;

            switch (targetType)
            {
                case ECreatureType.Player:
                    if (_playerCombatant.Tile.TilePosition == tile && !_playerCombatant.StatusReadOnly.HasStatus(ECreatureStatus.Hide))
                    {
                        combatant = _playerCombatant;
                        return true;
                    }
                    break;
                case ECreatureType.Enemy:
                    if (_enemyCombatProvider.TryGetCombatant(tile, out ICombatant combat))
                    {
                        if (!combat.StatusReadOnly.HasStatus(ECreatureStatus.Hide))
                        {
                            combatant = combat;
                            return true;
                        }
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
                Tile search = new Tile(pivot.X + positions[i, 0], pivot.Z + positions[i, 1]);

                if (_enemyCombatProvider.TryGetCombatant(search, out ICombatant combat)) count++;
            }

            return count;
        }
    }
}
