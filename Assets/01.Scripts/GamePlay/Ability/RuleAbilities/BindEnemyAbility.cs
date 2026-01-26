using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    using JW.DungeonSliding.GamePlay.Combat;
    using JW.DungeonSliding.GamePlay.Entities;

    public class BindEnemyAbility : RuleAbility
    {
        HashSet<ICombatant> _bindEenmies = new HashSet<ICombatant>();
        ICombatant _combatant;
        public BindEnemyAbility(RuleAbilityData data, IAbilityEntity entity) : base(data, entity) { }

        public override void ExcuteAbility()
        {
            if (!_bindEenmies.Contains(_combatant.AttackTarget))
            {
                _bindEenmies.Add(_combatant.AttackTarget);
                _combatant.AttackTarget.ApplyBind(ECreatureStatus.Bind, 1);
            }
        }

        public override void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if (triggerType == EAbilityTriggerType.Attack || triggerType == EAbilityTriggerType.BackAttack)
            {
                ExcuteAbility();
            }
        }
    }
}