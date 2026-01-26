
namespace JW.DungeonSliding.GamePlay.Combat
{
    [System.Serializable]
    public struct DamageInfo
    {
        public ICombatant Attacker;
        public int Damage;
        public bool IsCritical;
        public EStatusEffectType StatusEffect;
        public int StatusAmount;
        public DamageInfo(ICombatant attacker, int amount, bool isCritical, EStatusEffectType effectType = EStatusEffectType.None, int statusAmount = 0)
        {
            Attacker = attacker;
            Damage = amount;
            IsCritical = isCritical;
            StatusEffect = effectType;
            StatusAmount = statusAmount;
        }
    }
}
