using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class NextAttackEffect : IStatAbilityEffect
    {
        private INextAttackEnhancer _nextAttackEnhancer;
        private StatAbilityData _data;

        public NextAttackEffect(INextAttackEnhancer nextAttackEnhancer, StatAbilityData data)
        {
            _nextAttackEnhancer = nextAttackEnhancer;
            _data = data;
        }

        public void Apply()
        {
            switch (_data.NextAttackType)
            {
                case ENextAttackType.Add:
                    _nextAttackEnhancer.AddNextAttackDamage((Mathf.RoundToInt(_data.NextAttackValue)));
                    break;
                case ENextAttackType.Multiple:
                    _nextAttackEnhancer.AddNextAttackDamageMulti(_data.NextAttackValue);
                    break;
                case ENextAttackType.ExtraAttack:
                    _nextAttackEnhancer.AddNextAttackCount(Mathf.RoundToInt(_data.NextAttackValue));
                    break;
            }
        }

        public void Reset() { }
    }
}
