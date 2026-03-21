
using JW.DungeonSliding.GamePlay.Entities;
using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Combat
{
    [System.Serializable]
    public class DamageContext
    {
        public ICombatant Attacker = null;
        public int Damage = 0;
        public bool IsBackAttack = false;
        public bool IsCritical = false;
        public bool IsCounterAttack = false;

        public int AppliedFinalDamage { get; set; }

        private Dictionary<ECreatureStatus, int> _statusMap = new();
        public IReadOnlyDictionary<ECreatureStatus, int> Status => _statusMap;

        public void Clear()
        {
            Attacker = null;
            Damage = 0;
            IsCritical = false;
            _statusMap.Clear();
        }

        public void AddStatus(ECreatureStatus statusType, int amount)
        {
            if (!_statusMap.ContainsKey(statusType))
                _statusMap.Add(statusType, 0);

            _statusMap[statusType] += amount;
        }
    }
}
