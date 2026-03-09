using UnityEngine;
using JW.DungeonSliding.GamePlay.Entities;
using System;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class AttackRequester : IAttackRequester
    {
        private ICombatant _owner;
        private ECreatureType _ownerType;
        
        public event Action<ActPair> OnRequestAttack;
        public event Action<ActPair> OnRequestCounterAttack;

        public AttackRequester(ICombatant owner, ECreatureType myType)
        {
            _owner = owner;
            _ownerType = myType;
        }

        public void RequestCounterAttack(ICombatant target)
        {
            OnRequestCounterAttack?.Invoke(new ActPair(_owner, target, EAttackType.Counter));
        }

        public bool TrySubmitAttackRequest(ICombatantSensor sensor)
        {
            if (_owner.StatusReadOnly.HasStatus(ECreatureStatus.Stun)) return false;

            ECreatureType searchType = _ownerType == ECreatureType.Player ? ECreatureType.Enemy : ECreatureType.Player;

            if (sensor.GetCombatant(_owner.Tile.TilePosition.GetNextTileByDir(_owner.Rotate.Direction), searchType, out var target))
            {
                OnRequestAttack?.Invoke(new ActPair(_owner, target, EAttackType.Nomal));
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
