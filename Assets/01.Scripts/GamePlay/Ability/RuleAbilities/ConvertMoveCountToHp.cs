using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertMoveCountToHp : RuleAbility
    {
        ICombatant _combatant;
        IMoveable _moveable;
        public ConvertMoveCountToHp(RuleAbilityData data, IAbilityHost host) : base(data, host)
        {
            BindService<ICombatant>(ref _combatant);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            if (_moveable.CurrentMoveCount > _data.CostValue)
            {
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, _data.GainValue, EPlayerStat.None));
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, -_data.CostValue, EPlayerStat.None));
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