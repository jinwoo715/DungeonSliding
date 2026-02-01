using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertHpToMoveCount : RuleAbility
    {
        IPlayerStatReadOnly _statReadOnly;
        IPlayerStatModifier _statModifier;
        IMoveable _moveable;
        public ConvertHpToMoveCount(RuleAbilityData data, IAbilityHost host) : base(data, host)
        {
            BindService<IPlayerStatReadOnly>(ref _statReadOnly);
            BindService<IPlayerStatModifier>(ref _statModifier);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            if (_statReadOnly.Get(EPlayerStat.HP) > _data.CostValue)
            {
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, -_data.CostValue, EPlayerStat.None));
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, _data.GainValue, EPlayerStat.None));
            }
        }
        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnDeathByMoveCount)
            {
                ExcuteAbility();
            }
        }
    }
}
