using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class RuleAbilityBase : IAbility
    {
        public readonly RuleAbilityData _data;
        private IAbilityContextService _context;

        public EGameEventTrigger GameTrigger => _data.GameTrigger;
        public ECreatureTrigger CreatureTrigger => _data.CreatureTrigger;

        public RuleAbilityBase(RuleAbilityData data, IAbilityContextService context)
        {
            _data = data;
            _context = context;

            BindService();
            InitData();
        }
        protected virtual void InitData() { }
        public abstract IEnumerator Execute(AbilityArgs args);
        public virtual void ReleaseAbility() { }
        protected abstract void BindService();
        public void BindService<T>(ref T service) where T : class
        {
            if (_context.TryGet<T>(out var getService))
            {
                service = getService;
            }
            else
            {
                service = null;
                Debug.LogError($"Not Found Type {typeof(T)}");
            }
        }
    }
}