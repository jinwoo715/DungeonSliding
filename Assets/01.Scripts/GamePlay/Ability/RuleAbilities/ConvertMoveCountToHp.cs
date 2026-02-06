using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertMoveCountToHp : RuleAbility
    {
        Combat.IPlayerStatModifier _playerStatModifier;
        Stats.IPlayerStatProvider _statReadOnly;
        public ConvertMoveCountToHp(RuleAbilitySOData data, IAbilityHost host) : base(data, host)
        {
            BindService(ref _playerStatModifier);
            BindService(ref _statReadOnly);
        }

        public override void ExcuteAbility()
        {
            if (_statReadOnly.Get(EPlayerStatType.MoveCount) > _data.CostValue)
            {
                _playerStatModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.HP, EApplyStatType.Add, _data.GainValue, EPlayerStatType.None));
                _playerStatModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.MoveCount, EApplyStatType.Add, -_data.CostValue, EPlayerStatType.None));
            }
        }
        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnDeathByHP)
            {
                ExcuteAbility();
            }
        }
    }
}