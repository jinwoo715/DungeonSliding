using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface IRequesterRegistry
    {
        public void RegisterEnemyAttackRequester(IAttackRequester requester);
        public void UnRegisterEnemyAttackRequester(IAttackRequester requester);
        public void RegisterPlayerAttackRequester(IAttackRequester requester);
        public void UnRegisterPlayerAttackRequester(IAttackRequester requester);
    }

    public interface IRequesterProvider
    {
        public IAttackRequester PlayerRequester { get; }
        public IReadOnlyList<IAttackRequester> EnemyRequesters { get; }
    }
    public class FieldAttackRequesterManager : IRequesterProvider, IRequesterRegistry
    {
        private IAttackRequester _playerRequester;
        private List<IAttackRequester> _enemyRequesters = new();

        public IAttackRequester PlayerRequester => _playerRequester;
        public IReadOnlyList<IAttackRequester> EnemyRequesters => _enemyRequesters;


        public void RegisterPlayerAttackRequester(IAttackRequester requester)
        {
            _playerRequester = requester;
        }
        public void UnRegisterPlayerAttackRequester(IAttackRequester requester)
        {
            _playerRequester = null;
        }
        public void RegisterEnemyAttackRequester(IAttackRequester requester)
        {
            if (!_enemyRequesters.Contains(requester))
                _enemyRequesters.Add(requester);
        }
        public void UnRegisterEnemyAttackRequester(IAttackRequester requester)
        {
            if (_enemyRequesters.Contains(requester))
                _enemyRequesters.Remove(requester);
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
