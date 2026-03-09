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
    public interface IAbilityRandomGetter
    {
        void GetRandomAbility(int count);
    }

    public interface IAbilityContextService
    {
        bool TryGet<T>(out T service) where T : class;
        public void Register<T>(T service) where T : class;
    }

    public class PlayerAbilityContext : IAbilityContextService
    {
        private ICombatant _owner;
        private readonly Dictionary<Type, object> _services = new();

        public void SetOwner(ICombatant owner)
        {
            _owner = owner;
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

            service = _owner as T;
            return service != null;
        }
    }

    //어빌리티 추가
    //
    public interface IAbilityEventService
    {
        event Action<AbilityDataBase> OnAddedAbility;
        event Action<AbilitySession> OnExcuteAbilitySelection;
    }

    public class AbilitySystem : IRerollService, IAbilityRandomGetter, IAbilityEventService
    {
        private ShuffleBag<AbilityDataBase> _abilityBag;
        private Dictionary<string, AbilityDataBase> _abilityDataByUID = new();
        
        private AbilityFactory _abilityFactory = new AbilityFactory();

        public event Action<AbilityDataBase> OnAddedAbility;
        public event Action<AbilitySession> OnExcuteAbilitySelection;
        
        private IAbilityContextService _abilityHost;

        private ILevelProgress _playerLevel;
        private IAbilityRegister _abilityRegister;

        private int _rerollCount = 1;

        public void Init(IAbilityContextService abilityHost, ILevelProgress playerLevel, IAbilityRegister abilityRegister)
        {
            _abilityHost = abilityHost;
            _abilityHost.Register<IRerollService>(this);
            _abilityHost.Register<IAbilityRandomGetter>(this);

            LoadData();

            _playerLevel = playerLevel;
            _abilityRegister = abilityRegister;

            _playerLevel.OnLevelUp += ProcessAbilityLevel;
        }

        private void LoadData()
        {
            List<AbilityDataBase> datas = GameManager.Data.Abilities;

            _abilityBag = new ShuffleBag<AbilityDataBase>(datas);

            for (int i = 0; i < datas.Count; i++)
            {
                _abilityDataByUID.Add(datas[i].UID, datas[i]);
            }
        }

        private void ProcessAbilityLevel()
        {
            if(IsAchieveAbilityLevel())
            {
                RequestAbilitySelect();
            }
        }
        private bool IsAchieveAbilityLevel()
        {
            int abilityLevel = GameManager.Config.Ability.AbilityLevel;
            int currentLevel = _playerLevel.CurrentLevel;

            int achive = currentLevel % abilityLevel;

            return achive == 0;
        }
        public void RequestAbilitySelect()
        {
            var session = new AbilitySession(GetRandomAbilities(3), SelectAbility, () => GetRandomAbilities(3), _rerollCount);

            OnExcuteAbilitySelection?.Invoke(session);
        }
        public AbilityDataBase[] GetRandomAbilities(int count)
        {
            AbilityDataBase[] abilityDatas = new AbilityDataBase[count];
            for (int i = 0; i < count; i++)
            {
                abilityDatas[i] = _abilityBag.GetItem();
            }

            return abilityDatas;
        }
        public void SelectAbility(AbilityDataBase _abilityData)
        {
            IAbility ability = _abilityFactory.CreateAbility(_abilityData, _abilityHost);

            _abilityRegister.RegisterAbility(ability);

            OnAddedAbility?.Invoke(_abilityData);
        }
        public void AddReroll(int amount = 1)
        {
            _rerollCount += amount;
        }

        public void GetRandomAbility(int count)
        {
            AbilityDataBase[] abilities = new AbilityDataBase[count];
            abilities = GetRandomAbilities(count);

            for (int i = 0; i < count; i++)
            {
                SelectAbility(abilities[i]);
            }
        }
    }
}
