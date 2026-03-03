using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SurroundEmpowerAbility : AbilityBase
    {
        private ICombatantSensor _sensor;
        private ICombatant _combatant;
        private INextAttackEnhancer _nextAttackEnhancer;
        private IMoveable _moveable;
        private IPlayerStatModifier _modifier;

        private PlayerApplyStatContext _applyStatContext;

        public SurroundEmpowerAbility(RuleAbilityData data, AbilityHost host) : base(data, host) 
        {
            _applyStatContext = new PlayerApplyStatContext(EPlayerStatType.Damage, EApplyStatType.Add, EPlayerStatType.None, 0);
        }

        public override void ExcuteAbility()
        {
            int count = _sensor.GetNearEnemyCount(_combatant.Tile.TilePosition);

            _applyStatContext.AddValue(-_data.P1 * count);
            
            PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(EPlayerStatType.Damage, EApplyStatType.Add, EPlayerStatType.None, _data.P1 * count);
            _modifier.ModifyStat(applyStatContext);

            _nextAttackEnhancer.AddEnhance(ENextAttackType.Add, count);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (triggerType == EGameTriggerType.OnMoveEnd)
            {
                _modifier.ModifyStat(_applyStatContext);
                _applyStatContext.Reset();

                ExcuteAbility();
            }
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