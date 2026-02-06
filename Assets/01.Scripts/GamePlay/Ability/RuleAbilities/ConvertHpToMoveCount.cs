using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertHpToMoveCount : RuleAbility
    {
        Stats.IPlayerStatProvider _statReadOnly;
        Combat.IPlayerStatModifier _statModifier;
        IMoveable _moveable;
        public ConvertHpToMoveCount(RuleAbilitySOData data, IAbilityHost host) : base(data, host)
        {
            BindService(ref _statReadOnly);
            BindService(ref _statModifier);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            if (_statReadOnly.Get(EPlayerStatType.HP) > _data.CostValue)
            {
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.HP, EApplyStatType.Add, -_data.CostValue, EPlayerStatType.None));
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.MoveCount, EApplyStatType.Add, _data.GainValue, EPlayerStatType.None));
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
