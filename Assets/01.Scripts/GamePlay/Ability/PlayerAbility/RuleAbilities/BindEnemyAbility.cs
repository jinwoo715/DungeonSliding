using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    using JW.DungeonSliding.GamePlay.Combat;
    using JW.DungeonSliding.GamePlay.Entities;
    using System.Collections;

    public class BindEnemyAbility : RuleAbilityBase
    {
        HashSet<ICombatant> _bindEenmies = new HashSet<ICombatant>();
        ICombatant _combatant;
        public BindEnemyAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<ICombatant>(ref _combatant);
        }
    }
}