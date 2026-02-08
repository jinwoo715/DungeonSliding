namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class RuleAbilityBase : IAbility
    {
        public readonly RuleAbilityData _data;
        private AbilityHost _host;
        public EGameTriggerType ProgTriggers => _data.TriggerType;

        public RuleAbilityBase(RuleAbilityData data, AbilityHost host)
        {
            _data = data;
            _host = host;

            BindService();

            if (data.TriggerType == EGameTriggerType.Instant)
                ExcuteAbility();
        }

        protected abstract void BindService();

        public abstract void ExcuteAbility();
        public abstract void ProcTrigger(EGameTriggerType triggerType);
        public void BindService<T>(ref T service) where T : class
        {
            if (_host.TryGet<T>(out var getService))
            {
                service = getService;
            }
        }
    }
}