namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class RuleAbility : IAbility
    {
        public readonly RuleAbilityData _data;
        public IAbilityEntity Entity { get; private set; }

        public RuleAbility(RuleAbilityData data, IAbilityEntity entity)
        {
            _data = data;
            Entity = entity;
        }

        public abstract void ExcuteAbility();
        public abstract void ProcTrigger(EGameTriggerType triggerType);
    }
}