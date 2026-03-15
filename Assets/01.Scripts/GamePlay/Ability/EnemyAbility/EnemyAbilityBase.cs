using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class EnemyAbilityBase : IAbility
    {
        public EnemyAbilityData _data;
        IAbilityContextService _contextService;
        protected ICombatant _owner;

        public float P1 { get; private set; }
        public float P2 { get; private set; }
        public EGameEventTrigger GameTrigger => _data.GameTriggerType;
        public ECreatureTrigger CreatureTrigger => _data.CretureTriggerType;
        public EnemyAbilityBase(EnemyAbilityData data, IAbilityContextService contextService, ICombatant owner, int section)
        {
            _data = data;
            _contextService = contextService;
            _owner = owner;

            CalculateParam(section);
            BindService();
        }
        public abstract IEnumerator Execute(AbilityArgs args);
        public virtual void ReleaseAbility() { }
        protected abstract void BindService();
        public void BindService<T>(ref T service) where T : class
        {
            if (_contextService.TryGet<T>(out var getService))
            {
                service = getService;
            }
            else
            {
                if(_owner.TryGet<T>(out var ownerService))
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
        private void CalculateParam(int section)
        {
            P1 = _data.BaseP1 + (_data.GrowthP1 * section);
            P2 = _data.BaseP2 + (_data.GrowthP2 * section);
        }
    }
}
