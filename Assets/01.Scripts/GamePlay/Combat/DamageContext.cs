
namespace JW.DungeonSliding.GamePlay.Combat
{
    [System.Serializable]
    public struct DamageContext
    {
        public ICombatant Attacker;
        public int Damage;
        public bool IsCritical;
        public EStatusEffectType StatusEffect;
        public int StatusAmount;
        public DamageContext(ICombatant attacker, int amount, bool isCritical, EStatusEffectType effectType = EStatusEffectType.None, int statusAmount = 0)
        {
            Attacker = attacker;
            Damage = amount;
            IsCritical = isCritical;
            StatusEffect = effectType;
            StatusAmount = statusAmount;
        }

        public void Reset()
        {
            Attacker = null;
            Damage = 0;
            IsCritical = false;
            StatusEffect = EStatusEffectType.None;
            StatusAmount = 0;
        }
    }
}
