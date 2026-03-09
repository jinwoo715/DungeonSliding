using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class TriggerStatAbilty : IAbilityBase
    {
        public readonly TriggerStatAbiltyData _data;

        private INextAttackEnhancer _nextAttackEnhancer;
        public TriggerStatAbilty(IAbilityContextService host, TriggerStatAbiltyData data)
        {
            if (host.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))
                _nextAttackEnhancer = nextAttackEnhancer;

            _data = data;
        }

        public EGameEventTrigger ProgTriggers => throw new System.NotImplementedException();

        public void ExcuteAbility()
        {
            switch (_data.ApplyStatType)
            {
                case EAbilityApplyStatType.PlayerStat:

               //     PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
               //_data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);

               //     _modifier.ModifyStat(applyStatContext);

                    break;
                case EAbilityApplyStatType.NextAttack:

                    switch (_data.ApplyType)
                    {
                        case EApplyStatType.Add:
                            _nextAttackEnhancer.AddNextAttackDamage(Mathf.RoundToInt(_data.Value));
                            break;
                        case EApplyStatType.Multiple:
                            _nextAttackEnhancer.AddNextAttackDamageMulti(_data.Value);
                            break;
                    }

                    break;
            }
           
        }

        public void ProcTrigger(EGameEventTrigger triggerType)
        {
            ExcuteAbility();
        }
    }
}
