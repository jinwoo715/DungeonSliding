using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class SurroundEmpowerAbility : RuleAbility
    {
        private ICombatantSensor _sensor;
        private ICombatant _combatant;
        private IMoveable _moveable;
        public SurroundEmpowerAbility(RuleAbilityData data, IAbilityHost host) : base(data, host) 
        {
            BindService<ICombatantSensor>(ref _sensor);
            BindService<ICombatant>(ref _combatant);
            BindService<IMoveable>(ref _moveable);
        }

        public override void ExcuteAbility()
        {
            int count = _sensor.GetNearCambatantCount(_combatant);
            _combatant.AttackBuff.AddDamage(count);
        }

        public override void ProcTrigger(EGameTriggerType triggerType)
        {
            if (_moveable.SlideResultType == ESlideResultType.EnemyStop)
            {
                ExcuteAbility();
            }
        }
    }
}