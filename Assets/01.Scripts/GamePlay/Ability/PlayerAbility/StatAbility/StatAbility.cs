using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class StatAbility : IAbility
    {
        private readonly IStatAbilityEffect _effect;

        public EGameEventTrigger GameTrigger => EGameEventTrigger.None;
        public ECreatureTrigger CreatureTrigger => ECreatureTrigger.OnAdded;

        public StatAbility(StatAbilityData data, IAbilityContextService context)
        {
            if (context.TryGet<IStatModifier>(out var modifier))
            {
                _effect = new PlayerStatEffect(modifier, data);
            }
            else
            {
                Debug.LogError("IStatModifier Not Exist");
            }
        }

        public IEnumerator Execute(AbilityArgs args)
        {
            _effect?.Apply();
            yield return null;
        }

        public void ReleaseAbility()
        {
            _effect?.Reset();
        }
    }

    public class RuleStatAbility : IAbility
    {
        private readonly RuleStatAbilityData _data;
        private readonly IStatAbilityEffect _effect;

        private int _currentStack;
        private int _currentResetTriggerThreshold;

        public EGameEventTrigger GameTrigger => _data.GameTriggerType | _data.ResetGameTrigger;
        public ECreatureTrigger CreatureTrigger => _data.CreatureTriggerType | _data.ResetCreatureTrigger;

        public RuleStatAbility(RuleStatAbilityData data, IAbilityContextService context)
        {
            _data = data;
            _effect = CreateEffect(data, context);
        }

        private IStatAbilityEffect CreateEffect(RuleStatAbilityData data, IAbilityContextService context)
        {
            if (data.StatType == EAbilityApplyStatType.PlayerStat)
            {
                if (context.TryGet<IStatModifier>(out var modifier))
                    return new PlayerStatEffect(modifier, data);

                Debug.LogError("IStatModifier Not Exist");
                return null;
            }

            if (data.StatType == EAbilityApplyStatType.NextAttack)
            {
                if (context.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))
                    return new NextAttackEffect(nextAttackEnhancer, data);

                Debug.LogError("INextAttackEnhancer Not Exist");
                return null;
            }

            Debug.LogError($"Unsupported RuleStatAbility effect type : {data.StatType}");
            return null;
        }

        public IEnumerator Execute(AbilityArgs args)
        {
            if (args.GameTrigger == _data.GameTriggerType || args.CreatureTrigger == _data.CreatureTriggerType)
            {
                _currentStack++;

                if (_currentStack >= _data.NeedStackCount)
                {
                    _effect?.Apply();
                    _currentStack = 0;
                    _currentResetTriggerThreshold = 0;
                }
            }

            if (_data.IsResetEnabled)
            {
                if (args.GameTrigger == _data.ResetGameTrigger || args.CreatureTrigger == _data.ResetCreatureTrigger)
                {
                    _currentResetTriggerThreshold++;

                    if (_currentResetTriggerThreshold > _data.ResetThreshold)
                    {
                        ReleaseAbility();
                    }
                }
            }

            yield return null;
        }

        public void ReleaseAbility()
        {
            _currentStack = 0;
            _currentResetTriggerThreshold = 0;
            _effect?.Reset();
        }
    }
}