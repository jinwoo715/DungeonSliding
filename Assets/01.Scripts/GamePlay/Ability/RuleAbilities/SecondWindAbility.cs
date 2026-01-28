using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SecondWindAbility : RuleAbility
    {
        ICombatant _combatant;
        public SecondWindAbility(RuleAbilityData data, Player player) : base(data, player)
        {
        }

        public override void ExcuteAbility()
        {
            if (_combatant.CurrentHP > _data.CostValue)
            {
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, -_data.CostValue, EPlayerStat.None));
                _combatant.ModifyStat(new ApplyStatContext(EPlayerStat.MoveCount, EApplyStatType.Add, _data.GainValue, EPlayerStat.None));
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