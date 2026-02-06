using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class TriggerStatAbilty : IAbility
    {
        public readonly TriggerStatAbiltyData _data;

        private IPlayerStatModifier _modifier;
        private INextAttackEnhancer _nextAttackEnhancer;
        public TriggerStatAbilty(IAbilityHost host, TriggerStatAbiltyData data)
        {
            if (host.TryGet<IPlayerStatModifier>(out var modifier))
                _modifier = modifier;

            if (host.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))
                _nextAttackEnhancer = nextAttackEnhancer;

            _data = data;
        }

        public void ExcuteAbility()
        {
            switch (_data.ApplyStatType)
            {
                case EAbilityApplyStatType.EntityStat:

                    PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
               _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);

                    _modifier.ModifyStat(applyStatContext);

                    break;
                case EAbilityApplyStatType.NextActStat:

                    switch (_data.ApplyType)
                    {
                        case EApplyStatType.Add:
                            _nextAttackEnhancer.AddEnhance(ENextAttackType.Add, _data.Value);
                            break;
                        case EApplyStatType.Multiple:
                            _nextAttackEnhancer.AddEnhance(ENextAttackType.Multiple, _data.Value);
                            break;
                    }

                    break;
            }
           
        }

        public void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}
