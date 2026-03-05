using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.GamePlay.Combat;
using JW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IRerollService
    {
        void AddReroll(int amount = 1);
    }

    public class AbilityHost : IAbilityHost
    {
        private readonly object _entity; // 보통 Player
        private readonly Dictionary<Type, object> _services = new();

        public AbilityHost(object entity)
        {
            _entity = entity;
        }

        public void Register<T>(T service) where T : class
            => _services[typeof(T)] = service;

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = _entity as T;
            return service != null;
        }
    }

    //어빌리티 추가
    //
    public interface IAbilityService
    {
        event Action<AbilityDataBase> OnAddAbilityEvent;
        event Action<AbilitySession> OnAbilitySelectEvent;
    }

    public class AbilitySystem : IRerollService, IAbilityService
    {
        private ShuffleBag<AbilityDataBase> _abilityBag;
        private Dictionary<string, AbilityDataBase> _abilityDataByUID = new();
        
        private Dictionary<EGameTriggerType, List<IAbilityBase>> _hasAbilitiesByTrigger = new Dictionary<EGameTriggerType, List<IAbilityBase>>();
        private Dictionary<IAbilityBase, List<EGameTriggerType>> _hasTriggersByAbility = new Dictionary<IAbilityBase, List<EGameTriggerType>>();

        public AbilityFactory _abilityFactory = new AbilityFactory();

        private int _maxRerollCount = 1;
        
        AbilityHost _abilityHost;

        public event Action<AbilityDataBase> OnAddAbilityEvent;
        public event Action<AbilitySession> OnAbilitySelectEvent;

        public AbilitySystem(ICombatantSensor combatantSensor, IAbilityHost host)
        {
            _abilityHost = new AbilityHost(host);
            _abilityHost.Register<IRerollService>(this);
            _abilityHost.Register<ICombatantSensor>(combatantSensor);

            Init();
        }

        public void Init()
        {
            List<AbilityDataBase> datas = GameManager.Data.Abilities;

            _abilityBag = new ShuffleBag<AbilityDataBase>(datas);

            for (int i = 0; i < datas.Count; i++)
            {
                _abilityDataByUID.Add(datas[i].UID, datas[i]);
            }

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnLevelUp, RequestAbilitySelect);
        }
        public void RequestAbilitySelect()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnShowAbility);
            var session = new AbilitySession(GetAbilityDataSet(), GrantAbility, GetAbilityDataSet, _maxRerollCount);

            OnAbilitySelectEvent?.Invoke(session);
        }

        public AbilityDataBase[] GetAbilityDataSet()
        {
            AbilityDataBase[] abilityDatas = new AbilityDataBase[3];
            for (int i = 0; i < 3; i++)
            {
                abilityDatas[i] = _abilityBag.GetItem();
            }

            return abilityDatas;
        }

        public void GrantAbility(string _abilityUID)
        {
            AbilityDataBase data = _abilityDataByUID[_abilityUID];
            IAbilityBase ability = _abilityFactory.CreateAbility(data, _abilityHost);

            EnrollAbility(ability.ProgTriggers, ability);

            OnAddAbilityEvent?.Invoke(data);

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnHideAbility);
        }

        private void EnrollAbility(EGameTriggerType types, IAbilityBase ability)
        {
            var triggers = SplitTriggers(types);

            foreach (var trigger in triggers)
            {
                if (!_hasAbilitiesByTrigger.ContainsKey(trigger))
                {
                    _hasAbilitiesByTrigger.Add(trigger, new List<IAbilityBase>());

                    GameTriggerEventBus.Instance.SubscribeTriggerEvent(trigger, () => { ExcuteAbility(trigger); });
                }

                Debug.Log($"{trigger}, {triggers}");
                _hasAbilitiesByTrigger[trigger].Add(ability);
            }
        }

        public void ExcuteAbility(EGameTriggerType trigger)
        {
            foreach (var ability in _hasAbilitiesByTrigger[trigger])
            {
                ability.ProcTrigger(trigger);
            }
        }

        public void AddReroll(int amount = 1)
        {
            Debug.Log("AddReroll");
            _maxRerollCount += amount;
        }

        public static IEnumerable<EGameTriggerType> SplitTriggers(EGameTriggerType triggers)
        {
            foreach (EGameTriggerType value in Enum.GetValues(typeof(EGameTriggerType)))
            {
                if (value == EGameTriggerType.None)
                    continue;

                if (triggers.HasFlag(value))
                    yield return value;
            }
        }
    }
}
