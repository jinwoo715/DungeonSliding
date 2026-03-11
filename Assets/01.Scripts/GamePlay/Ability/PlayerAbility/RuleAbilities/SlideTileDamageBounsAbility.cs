using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SlideTileDamageBounsAbility : RuleAbilityBase
    {
        INextAttackEnhancer _nextAttackEnhancer;
        IMoveable _moveable;
        public SlideTileDamageBounsAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            int moveTileCount = _moveable.SlideTileCount();
            _nextAttackEnhancer.AddNextAttackDamage(Mathf.RoundToInt(moveTileCount * _data.P1));
        }

        public override void ProcTrigger(EGameEventTrigger triggerType)
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