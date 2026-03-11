using System.Collections;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class RuleAbilityBase : IAbility
    {
        public readonly RuleAbilityData _data;
        private IAbilityContextService _context;
        public EGameEventTrigger ProgTriggers => _data.TriggerType;

        public EGameEventTrigger GameTrigger => throw new System.NotImplementedException();

        public ECreatureTrigger CreatureTrigger => throw new System.NotImplementedException();

        public RuleAbilityBase(RuleAbilityData data, IAbilityContextService context)
        {
            _data = data;
            _context = context;

            BindService();
        }

        protected abstract void BindService();

        public abstract void ExcuteAbility();
        public abstract void ProcTrigger(EGameEventTrigger triggerType);
        public void BindService<T>(ref T service) where T : class
        {
            if (_context.TryGet<T>(out var getService))
            {
                service = getService;
            }
        }

        public IEnumerator Execute()
        {
            throw new System.NotImplementedException();
        }

        public void ReleaseAbility()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerator Execute(AbilityArgs args)
        {
            throw new System.NotImplementedException();
        }
    }
}