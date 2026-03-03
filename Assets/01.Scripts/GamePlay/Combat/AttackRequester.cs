using UnityEngine;
using JW.DungeonSliding.GamePlay.Entities;
namespace JW.DungeonSliding.GamePlay.Combat
{
    public class AttackRequester : IAttackRequester
    {
        private ICombatant _owner;
        private ECreatureType _myType;

        public AttackRequester(ICombatant owner, ECreatureType myType)
        {
            _owner = owner;
            _myType = myType;
        }

        public bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener)
        {
            if (_owner.StatusReadOnly.HasStatus(ECreatureStatus.Stun)) return false;

            ECreatureType searchType = _myType == ECreatureType.Player ? ECreatureType.Enemy : ECreatureType.Player;

            if (sensor.GetCombatant(_owner.Tile.TilePosition.GetNextTileByDir(_owner.Rotate.Direction), searchType, out var target))
            {
                attackRequestListener.EnqueueActPair(new ActPair(_owner, target));
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
