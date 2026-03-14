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
        public abstract IEnumerator Execute(AbilityArgs args);
        protected abstract void BindService();
        protected virtual void InitData() { }
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
        public virtual void ReleaseAbility() { }
        public bool IsCheckChanceSuccess(float chance)
        {
            int ranNum = UnityEngine.Random.Range(1, 101);
            return chance >= ranNum;
        }
    }
}