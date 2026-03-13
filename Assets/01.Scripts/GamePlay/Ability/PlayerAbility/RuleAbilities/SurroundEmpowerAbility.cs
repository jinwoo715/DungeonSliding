using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SurroundEmpowerAbility : RuleAbilityBase
    {
        private ICombatantSensor _sensor;
        private ICombatant _combatant;
        private INextAttackEnhancer _nextAttackEnhancer;
        private IMoveable _moveable;
        private IPlayerStatModifier _modifier;

        private ApplyStatContext _applyStatContext;

        public SurroundEmpowerAbility(RuleAbilityData data, IAbilityContextService host) : base(data, host) 
        {
            //_applyStatContext = new PlayerApplyStatContext(EPlayerStatType.Damage, EApplyStatType.Add, EPlayerStatType.None, 0);
        }

        public override IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }

        protected override void BindService()
        {
            BindService<ICombatantSensor>(ref _sensor);

            Debug.Log(_sensor);

            BindService<ICombatant>(ref _combatant);
            BindService<IMoveable>(ref _moveable);
            BindService<INextAttackEnhancer>(ref _nextAttackEnhancer);
            BindService(ref _modifier);
        }
    }
}