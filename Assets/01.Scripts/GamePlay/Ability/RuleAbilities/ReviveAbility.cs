using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ReviveAbility : RuleAbility
    {
        private IStatModifier _statModifier;
        private bool isCunsumed = false;
        public ReviveAbility(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            BindService<IStatModifier>(ref _statModifier);
        }

        public override void ExcuteAbility()
        {
            ApplyStatContext hp = new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Ratio, 0.5f, EPlayerStat.MaxHp);
            ApplyStatContext move = new ApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Ratio, 0.5f, EPlayerStat.MaxMoveCount);
            _statModifier.ModifyStat(hp);
            _statModifier.ModifyStat(move);

            isCunsumed = true;
        }
        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (isCunsumed == true) return;
                
            ExcuteAbility();
        }
    }
}
