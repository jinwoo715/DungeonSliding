using UnityEngine;
using JW.DungeonSliding.GamePlay.Entities;
namespace JW.DungeonSliding.GamePlay.Combat
{
    public class AttackRequester : IAttackRequester
    {
        private ICombatant _owner;
        private ECreatureType _myType;
        private IAttackRequestListener _attackRequestListener;

        public AttackRequester(ICombatant owner, ECreatureType myType, IAttackRequestListener attackRequestListener)
        {
            _owner = owner;
            _myType = myType;
            _attackRequestListener = attackRequestListener;
        }

        public void RequestCounterAttack(ICombatant target)
        {
            _attackRequestListener.EnqueueCounterActPair(new ActPair(_owner, target));
        }

        public bool TrySubmitAttackRequest(ICombatantSensor sensor)
        {
            if (_owner.StatusReadOnly.HasStatus(ECreatureStatus.Stun)) return false;

            ECreatureType searchType = _myType == ECreatureType.Player ? ECreatureType.Enemy : ECreatureType.Player;

            if (sensor.GetCombatant(_owner.Tile.TilePosition.GetNextTileByDir(_owner.Rotate.Direction), searchType, out var target))
            {
                _attackRequestListener.EnqueueActPair(new ActPair(_owner, target));
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
