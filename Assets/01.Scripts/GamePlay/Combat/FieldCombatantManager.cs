using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface IRequesterRegistry
    {
        public void RegisterAttackRequester(IAttackRequester requester, int priority);
        public void UnRegisterAttackRequester(IAttackRequester requester, int priority);
    }

    public interface IRequesterProvider
    {
        public IReadOnlyDictionary<int, List<IAttackRequester>> RequesterByPriority { get; }
    }
    public class FieldAttackRequesterManager : IRequesterProvider, IRequesterRegistry
    {
        public IReadOnlyDictionary<int, List<IAttackRequester>> RequesterByPriority => _requesterByPriority;
        private SortedDictionary<int, List<IAttackRequester>> _requesterByPriority = new();
        public void RegisterAttackRequester(IAttackRequester requester, int priority)
        {
            if (!_requesterByPriority.ContainsKey(priority))
                _requesterByPriority.Add(priority, new List<IAttackRequester>());

            _requesterByPriority[priority].Add(requester);
        }
        public void UnRegisterAttackRequester(IAttackRequester requester, int priority)
        {
            if (!_requesterByPriority.TryGetValue(priority, out var list))
            {
                if (list.Contains(requester))
                    list.Remove(requester);
            }
        }
    }

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
        public bool GetCombatant(Tile tile, ECreatureType targetType, out ICombatant combatant)
        {
            switch (targetType)
            {
                case ECreatureType.Player:
                    if (_playerCombatant.Tile.TilePosition == tile)
                    {
                        combatant = _playerCombatant;
                        return true;
                    }
                    break;
                case ECreatureType.Enemy:
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
                Tile search = new Tile(pivot.X + positions[i, 0], pivot.Z + positions[i, 1]);

                if (_enemyCombatProvider.TryGetCombatant(search, out ICombatant combat)) count++;
            }

            return count;
        }
    }
}
