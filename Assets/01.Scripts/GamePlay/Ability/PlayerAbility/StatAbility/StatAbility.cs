using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class StatAbility : IAbility
    {
        private StatAbilityData _data;
        private IAbilityContextService _context;
        private IStatAbilityEffect _effect;

        private int _currentStack = 0;
        private int _currentResetTriggerThreshold = 0;

        public EGameEventTrigger GameTrigger => _data.GameTriggerType | _data.ResetGameTrigger;
        public ECreatureTrigger CreatureTrigger => _data.CreatureTriggerType | _data.ResetCreatureTrigger;

        //TODO 수정 필요
        public StatAbility(StatAbilityData data, IAbilityContextService context)
        {
            _data = data;
            _context = context;

            if (_data.StatType == EAbilityApplyStatType.PlayerStat)
            {
                if (_context.TryGet<IStatModifier>(out var modifier))
                {
                    _effect = new PlayerStatEffect(modifier, data);
                }
                else
                {
                    Debug.LogError("IStatModifier Not Exist");
                }
            }
            else if (_data.StatType == EAbilityApplyStatType.NextAttack)
            {
                if (_context.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))
                {
                    _effect = new NextAttackEffect(nextAttackEnhancer, data);
                }
                else
                {
                    Debug.LogError("INextAttackEnhancer Not Exist");
                }
            }
        }
        public IEnumerator Execute(AbilityArgs args)
        {
            if(args.GameTrigger == _data.GameTriggerType || args.CreatureTrigger == _data.CreatureTriggerType)
            {
                _currentStack++;

                if(_currentStack >= _data.NeedStackCount)
                {
                    _effect.Apply();
                    _currentStack = 0;
                    _currentResetTriggerThreshold = 0;
                }
            }

            if (_data.IsResetEnabled == true)
            {
                if (args.GameTrigger == _data.ResetGameTrigger || args.CreatureTrigger == _data.ResetCreatureTrigger)
                {
                    _currentResetTriggerThreshold++;

                    if(_currentResetTriggerThreshold > _data.ResetThreshold)
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
            _effect.Reset();
        }
    }
}
