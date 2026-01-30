namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class RuleAbility : IAbility
    {
        public readonly RuleAbilityData _data;
        public IAbilityHost Host { get; private set; }

        public RuleAbility(RuleAbilityData data, IAbilityHost host)
        {
            _data = data;
            Host = host;
        }

        public abstract void ExcuteAbility();
        public abstract void ProcTrigger(EGameTriggerType triggerType);
        public void BindService<T>(ref T service) where T : class
        {
            if (Host.TryGet<T>(out var getService))
            {
                service = getService;
            }
        }
    }
}