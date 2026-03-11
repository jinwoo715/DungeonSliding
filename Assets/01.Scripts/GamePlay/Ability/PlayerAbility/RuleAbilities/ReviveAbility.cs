using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ReviveAbility : RuleAbilityBase
    {
        private IPlayerStatModifier _statModifier;
        private int isReviveCount = 0;
        public ReviveAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            float reviveStatValue = _data.P2 * 0.01f;
            //PlayerApplyStatContext hp = new PlayerApplyStatContext(EPlayerStatType.CurrentHP, EApplyStatType.Ratio, EPlayerStatType.MaxHp, reviveStatValue);
            //PlayerApplyStatContext move = new PlayerApplyStatContext(EPlayerStatType.CurrentMoveCount, EApplyStatType.Ratio, EPlayerStatType.MaxMoveCount, reviveStatValue);
            //_statModifier.SetCurrentHP(hp);
            //_statModifier.SetCurrentMoveCount(move);

            isReviveCount++;
        }
        public override void ProcTrigger(EGameEventTrigger triggerType)
        {
            if (isReviveCount >= _data.P1) return;
                
            ExcuteAbility();
        }

        protected override void BindService()
        {
            BindService<IPlayerStatModifier>(ref _statModifier);
        }
    }
}
