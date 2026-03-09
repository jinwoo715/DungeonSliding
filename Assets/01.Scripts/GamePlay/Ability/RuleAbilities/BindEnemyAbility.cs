using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    using JW.DungeonSliding.GamePlay.Combat;
    using JW.DungeonSliding.GamePlay.Entities;

    public class BindEnemyAbility : AbilityBase
    {
        HashSet<ICombatant> _bindEenmies = new HashSet<ICombatant>();
        ICombatant _combatant;
        public BindEnemyAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override void ExcuteAbility()
        {
            //if (!_bindEenmies.Contains(_combatant.AttackTarget))
            //{
            //    _bindEenmies.Add(_combatant.AttackTarget);
            //    //_combatant.AttackTarget.ApplyStatus(ECreatureStatus.Bind, 1);
            //}
        }

        public override void ProcTrigger(EGameEventTrigger triggerType)
        {
            //if (triggerType == EGameTriggerType.OnAttack)
            //{
            //    ExcuteAbility();
            //}

            if(triggerType == EGameEventTrigger.OnEnterRoom)
            {
                _bindEenmies.Clear();
            }
        }

        protected override void BindService()
        {
            BindService<ICombatant>(ref _combatant);
        }
    }
}