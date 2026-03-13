
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

        private Dictionary<EStatusEffectType, int> _statusMap = new();
        public IReadOnlyDictionary<EStatusEffectType, int> Status => _statusMap;

        public DamageContext(ICombatant attacker, int amount, bool isCritical)
        {
            Attacker = attacker;
            Damage = amount;
            IsCritical = isCritical;
            _statusMap = new();
        }
        public void AddStatus(EStatusEffectType statusType, int amount)
        {
            if (!_statusMap.ContainsKey(statusType))
                _statusMap.Add(statusType, 0);

            _statusMap[statusType] += amount;
        }
    }
}
