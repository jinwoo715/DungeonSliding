using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class ExtraAttackChance : AbilityBase
    {
        INextAttackEnhancer _service;
        IMoveable _moveable;
        public ExtraAttackChance(RuleAbilityData data, AbilityHost host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            Debug.Log("Extra Attack");
            _service.AddEnhance(ENextAttackType.ExtraAttack, _data.P2);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if(triggerType == EGameTriggerType.OnMoveEnd && _moveable.SlideResultType == ESlideResultType.EnemyStop)
            {
                int chanceValue = Random.Range(0, 101);

                Debug.Log(chanceValue);

                if (chanceValue <= _data.P1)
                {
                    ExcuteAbility();
                }
            }
        }

        protected override void BindService()
        {
            BindService<INextAttackEnhancer>(ref _service);
            BindService(ref _moveable);

        }
    }
}