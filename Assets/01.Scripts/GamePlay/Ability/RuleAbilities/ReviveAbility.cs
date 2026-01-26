namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ReviveAbility : RuleAbility
    {
        private bool isCunsumed = false;

        public ReviveAbility(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            ApplyStatContext hp = new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Ratio, 0.5f, EPlayerStat.MaxHp);
            ApplyStatContext move = new ApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Ratio, 0.5f, EPlayerStat.MaxMoveCount);
            Entity.ModifyStat(hp);
            Entity.ModifyStat(move);

            isCunsumed = true;
        }
        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (isCunsumed == true) return;

            if (triggerType == EAbilityTriggerType.OnDeathByHP || triggerType == EAbilityTriggerType.OnDeathByMoveCount)
            {
                ExcuteAbility();
            }
        }
    }
}
