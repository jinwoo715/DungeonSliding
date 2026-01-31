using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
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
        private readonly object _entity; // ∫∏≈Î Player
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

    public class AbilitySystem : IRerollService
    {
        private ShuffleBag<AbilityData> _abilityBag;
        private Dictionary<int, AbilityData> _abilityDataByUID = new();
        
        private Dictionary<EGameTriggerType, List<IAbility>> _hasAbilitiesByTrigger = new Dictionary<EGameTriggerType, List<IAbility>>();
        private Dictionary<IAbility, List<EGameTriggerType>> _hasTriggersByAbility = new Dictionary<IAbility, List<EGameTriggerType>>();

        public AbilityFactory _abilityFactory = new AbilityFactory();

        private IGameModeChanger _gameModeChanger;
        private IAbilitySelectService _selectServeice;
        private int _maxRerollCount = 1;
        
        AbilityHost _abilityHost;

        public AbilitySystem(IAbilitySelectService abilitySelectService, IGameModeChanger gameModeChanger, IAbilityHost host)
        {
            _selectServeice = abilitySelectService;
            _gameModeChanger = gameModeChanger;

            _abilityHost = new AbilityHost(host);
            _abilityHost.Register<IRerollService>(this);

            Init();
        }

        public void Init()
        {
            List<AbilityData> datas = GameManager.Instance.Resource.AllAbility;

            _abilityBag = new ShuffleBag<AbilityData>(datas);

            for (int i = 0; i < datas.Count; i++)
            {
                int index = i;
                datas[i].AbilityUID = index;
                _abilityDataByUID.Add(index, datas[index]);
            }

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.LevelUp, RequestAbilitySelect);
        }
        public void RequestAbilitySelect()
        {
            _gameModeChanger.EnterGameMode(EGameModeType.AbilityUI);
            var session = new AbilitySession(GetAbilityDataSet(), GrantAbility, GetAbilityDataSet, _maxRerollCount);
            _selectServeice.SetAbilitySession(session);
        }

        public AbilityData[] GetAbilityDataSet()
        {
            AbilityData[] abilityDatas = new AbilityData[3];
            for (int i = 0; i < 3; i++)
            {
                abilityDatas[i] = _abilityBag.GetItem();
            }

            return abilityDatas;
        }

        public void GrantAbility(int _abilityUID)
        {
            AbilityData data = _abilityDataByUID[_abilityUID];
            IAbility ability = _abilityFactory.CreateAbility(data, _abilityHost);

            EnrollAbility(data.GetEnrollTriggers, ability);

            _gameModeChanger.ExitGameMode(EGameModeType.AbilityUI);
        }

        private void EnrollAbility(List<EGameTriggerType> types, IAbility ability)
        {
            _hasTriggersByAbility.Add(ability, types);

            for (int i = 0; i < types.Count; i++)
            {
                EGameTriggerType type = types[i];

                if (!_hasAbilitiesByTrigger.ContainsKey(type))
                {
                    _hasAbilitiesByTrigger.Add(type, new List<IAbility>());
                }

                _hasAbilitiesByTrigger[type].Add(ability);
            }
        }

        public void AddReroll(int amount = 1)
        {
            _maxRerollCount += amount;
        }
    }
}
