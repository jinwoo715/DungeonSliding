using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    using JW.DungeonSliding.GamePlay.Combat;
    using JW.DungeonSliding.GamePlay.Entities;

    public class BindEnemyAbility : RuleAbility
    {
        HashSet<ICombatant> _bindEenmies = new HashSet<ICombatant>();
        ICombatant _combatant;
        public BindEnemyAbility(RuleAbilitySOData data, IAbilityHost host) : base(data, host) 
        {
            BindService<ICombatant>(ref _combatant);
        }

        public override void ExcuteAbility()
        {
            if (!_bindEenmies.Contains(_combatant.AttackTarget))
            {
                _bindEenmies.Add(_combatant.AttackTarget);
                _combatant.AttackTarget.ApplyStatus(ECreatureStatus.Bind, 1);
            }
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnAttack)
            {
                ExcuteAbility();
            }

            if(triggerType == EGameTriggerType.OnEnterRoom)
            {
                _bindEenmies.Clear();
            }
        }
    }
}