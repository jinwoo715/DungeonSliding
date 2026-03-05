using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideTileDamageBounsAbility : AbilityBase
    {
        INextAttackEnhancer _nextAttackEnhancer;
        IMoveable _moveable;
        public SlideTileDamageBounsAbility(RuleAbilityData data, AbilityHost host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            int moveTileCount = _moveable.SlideTileCount();
            _nextAttackEnhancer.AddDamage(Mathf.RoundToInt(moveTileCount * _data.P1));
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if(_moveable.SlideResultType == ESlideResultType.EnemyStop)
                ExcuteAbility();
        }

        protected override void BindService()
        {
            BindService<INextAttackEnhancer>(ref _nextAttackEnhancer);
            BindService<IMoveable>(ref _moveable);
        }
    }
}