using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class BloodDriveAbility : RuleAbility
    {
        ICombatant _combatant;
        IMoveable _moveable;
        public BloodDriveAbility(RuleAbilityData data, Player player) : base(data, player)
        {
        }

        public override void ExcuteAbility()
        {
            if (_moveable.CurrentMoveCount > _data.CostValue)
            {
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, _data.GainValue, EPlayerStat.None));
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, -_data.CostValue, EPlayerStat.None));
            }
        }
        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.OnDeathByHP)
            {
                ExcuteAbility();
            }
        }
    }
}