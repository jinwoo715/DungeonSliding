using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertMoveCountToHp : AbilityBase
    {
        Combat.IPlayerStatModifier _playerStatModifier;
        Stats.IStatReadOnly _statReadOnly;
        public ConvertMoveCountToHp(RuleAbilityData data, IAbilityContextService host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility()
        {
            if (_statReadOnly.Get(ECreatureStatType.CurrentMoveCount) > _data.P1)
            {
                //_playerStatModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentMoveCount, EApplyStatType.Add, EPlayerStatType.None, -_data.P1));
                //_playerStatModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentHP, EApplyStatType.Add, EPlayerStatType.None, _data.P2));
            }
        }
        public override void ProcTrigger(EGameEventTrigger triggerType)
        {
            //if (triggerType == EGameEventTriggerType.OnDeathByHP)
            //{
            //    ExcuteAbility();
            //}
        }

        protected override void BindService()
        {
            BindService(ref _playerStatModifier);
            BindService(ref _statReadOnly);
        }
    }
}