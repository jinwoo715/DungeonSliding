using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ConvertHpToMoveCount : RuleAbilityBase
    {
        IPlayerStatProvider _statReadOnly;
        IPlayerStatModifier _statModifier;
        IMoveable _moveable;
        public ConvertHpToMoveCount(RuleAbilityData data, AbilityHost host) : base(data, host)
        {
            
        }

        public override void ExcuteAbility()
        {
            if (_statReadOnly.Get(EPlayerStatType.CurrentHP) > _data.P1)
            {
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentHP, EApplyStatType.Add, EPlayerStatType.None, -_data.P1));
                _statModifier.ModifyStat(new PlayerApplyStatContext(EPlayerStatType.CurrentMoveCount, EApplyStatType.Add, EPlayerStatType.None, _data.P2));
            }
        }
        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnDeathByMoveCount)
            {
                ExcuteAbility();
            }
        }

        protected override void BindService()
        {
            BindService(ref _statReadOnly);
            BindService(ref _statModifier);
            BindService<IMoveable>(ref _moveable);
        }
    }
}
