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
        void ObtainRandomRuleAbility(int count);
        void ObtainRandomStatAbility(int count);
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

    public interface IAbilityEventService
    {
        event Action<AbilityDataBase> OnAddedRuleAbility;
        event Action<IAbility> OnSelectAbility;
        event Action<AbilitySelectSession> OnExcuteAbilitySelection;
        void GrantAbilityPoint(int currentLevel);
    }


    public class PlayerAbilitySystem : IRerollService, IAbilityRandomGetter, IAbilityEventService
    {
        private ShuffleBag<AbilityDataBase> _ruleAbilityBag;
        private ShuffleBag<AbilityDataBase> _statAbilityBag;

        private Dictionary<string, AbilityDataBase> _statAbilityDatas = new();
        private Dictionary<string, AbilityDataBase> _ruleAbilityDatas = new();

        private IPlayerAbilityFactory _playerAbilityFactory;

        public event Action<AbilityDataBase> OnAddedRuleAbility;
        public event Action<IAbility> OnSelectAbility;
        public event Action<AbilitySelectSession> OnExcuteAbilitySelection;

        private int _rerollCount = 1;

        Queue<AbilitySelectSession> abilitySelectSessions = new Queue<AbilitySelectSession>();

        public void Init(IPlayerAbilityFactory playerAbilityFactory, IAbilityContextService abilityContext)
        {
            abilityContext.Register<IRerollService>(this);
            abilityContext.Register<IAbilityRandomGetter>(this);
            _playerAbilityFactory = playerAbilityFactory;

            LoadData();

            var session2 = new AbilitySelectSession(GetRandomRuleAbilities(3), SelectAbility, () => GetRandomRuleAbilities(3), _rerollCount);
            abilitySelectSessions.Enqueue(session2);

            GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnEnterRoom, ProgressAbilitySelect);
        }
        private void LoadData()
        {
            List<AbilityDataBase> sDatas = GameManager.Data.StatAbilities;
            List<AbilityDataBase> rDatas = GameManager.Data.RuleAbilities;

            _statAbilityBag = new ShuffleBag<AbilityDataBase>(sDatas);
            for (int i = 0; i < sDatas.Count; i++)
            {
                _statAbilityDatas.Add(sDatas[i].UID, sDatas[i]);
            }

            _ruleAbilityBag = new ShuffleBag<AbilityDataBase>(rDatas);
            for (int i = 0; i < rDatas.Count; i++)
            {
                _ruleAbilityDatas.Add(rDatas[i].UID, rDatas[i]);
            }
        }
        public void GrantAbilityPoint(int currentLevel)
        {
            Debug.Log($"GrantAbilityPoint : {currentLevel}");
            var session = new AbilitySelectSession(GetRandomStatAbilites(3), SelectAbility, () => GetRandomStatAbilites(3), _rerollCount);
            abilitySelectSessions.Enqueue(session);

            if (IsAbilityLevel(currentLevel))
            {
                var session2 = new AbilitySelectSession(GetRandomRuleAbilities(3), SelectAbility, () => GetRandomRuleAbilities(3), _rerollCount);
                abilitySelectSessions.Enqueue(session2);
            }

            GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnTurnEnd, ProgressAbilitySelect);
        }
        private bool IsAbilityLevel(int currentLevel)
        {
            int abilityLevel = GameManager.Config.Ability.AbilityLevel;

            int achive = currentLevel % abilityLevel;

            return achive == 0;
        }
        public void ProgressAbilitySelect()
        {
            Debug.Log(abilitySelectSessions.Count);
            if (IsExistGetAbility())
            {
                var session = abilitySelectSessions.Dequeue();
                session.OnEndSession += ProgressAbilitySelect;

                OnExcuteAbilitySelection?.Invoke(session);
            }
        }

        private bool IsExistGetAbility()
        {
            return abilitySelectSessions.Count > 0;
        }

        public void SelectAbility(AbilityDataBase _abilityData)
        {
            IAbility ability = _playerAbilityFactory.CreateAbility(_abilityData);

            OnAddedRuleAbility?.Invoke(_abilityData);
            OnSelectAbility?.Invoke(ability);
        }
        public AbilityDataBase[] GetRandomRuleAbilities(int count)
        {
            AbilityDataBase[] abilityDatas = new AbilityDataBase[count];
            for (int i = 0; i < count; i++)
            {
                abilityDatas[i] = _ruleAbilityBag.GetItem();
            }

            return abilityDatas;
        }
        public AbilityDataBase[] GetRandomStatAbilites(int count)
        {
            AbilityDataBase[] abilityDatas = new AbilityDataBase[count];
            for (int i = 0; i < count; i++)
            {
                abilityDatas[i] = _statAbilityBag.GetItem();
            }

            return abilityDatas;
        }
        public void ObtainRandomRuleAbility(int count)
        {
            var abilities = GetRandomRuleAbilities(count);

            for (int i = 0; i < count; i++)
            {
                SelectAbility(abilities[i]);
            }
        }
        public void ObtainRandomStatAbility(int count)
        {
            var abilities = GetRandomStatAbilites(count);

            for (int i = 0; i < count; i++)
            {
                SelectAbility(abilities[i]);
            }
        }
        public void AddReroll(int amount = 1)
        {
            _rerollCount += amount;
        }
    }
}
