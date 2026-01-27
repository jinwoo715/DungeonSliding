using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class FieldCombatantSensor : ICombatantSensor
    {
        private ICombatant _playerCombatant;
        private ICombatProvider _enemyCombatProvider;

        public FieldCombatantSensor(ICombatProvider combatProvider, ICombatant player)
        {
            _enemyCombatProvider = combatProvider;
            _playerCombatant = player;
        }

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

        public int GetNearCambatantCount(ICombatant except)
        {
            return 0;
        }
    }
}
