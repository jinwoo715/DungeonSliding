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
        private ShuffleBag<AbilityDataBase> _specialAbilityBag;
        private ShuffleBag<AbilityDataBase> _statAbilityBag;

        private Dictionary<string, AbilityDataBase> _statAbilityDatas = new();
        private Dictionary<string, AbilityDataBase> _specialAbilityDatas = new();

        private IPlayerAbilityFactory _playerAbilityFactory;

        public event Action<AbilityDataBase> OnAddedRuleAbility;
        public event Action<IAbility> OnSelectAbility;
        public event Action<AbilitySelectSession> OnExcuteAbilitySelection;

        private int _rerollCount = 10;

        Queue<AbilitySelectSession> abilitySelectSessions = new Queue<AbilitySelectSession>();

        public void Init(IPlayerAbilityFactory playerAbilityFactory, IAbilityContextService abilityContext)
        {
            abilityContext.Register<IRerollService>(this);
            abilityContext.Register<IAbilityRandomGetter>(this);
            _playerAbilityFactory = playerAbilityFactory;

            LoadData();

            var session = new AbilitySelectSession(GetRandomRuleAbilities(3), SelectRuleAbility, () => GetRandomRuleAbilities(3), _rerollCount);
            abilitySelectSessions.Enqueue(session);

            GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnEnterRoom, ProgressAbilitySelect);
        }
        private void LoadData()
        {
            if (GameManager.Data.StatAbilities == null ||
                GameManager.Data.StatAbilities.Count == 0)
            {
                Debug.LogWarning(
                    "Stat ability data is empty. Reloading ability data before initializing PlayerAbilitySystem.");
                GameManager.Data.ReloadAbilityData();
            }

            List<AbilityDataBase> sDatas = GameManager.Data.StatAbilities?.FindAll(
                data => data != null) ?? new List<AbilityDataBase>();
            List<AbilityDataBase> specialDatas = GameManager.Data.RuleAbilities?.FindAll(
                data => data != null) ?? new List<AbilityDataBase>();

            if (sDatas.Count == 0 || specialDatas.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Player ability initialization failed. Stat: {sDatas.Count}, Rule: {specialDatas.Count}");
            }

            _statAbilityBag = CreateRankWeightedBag(sDatas);
            for (int i = 0; i < sDatas.Count; i++)
            {
                _statAbilityDatas.Add(sDatas[i].UID, sDatas[i]);
            }

            _specialAbilityBag = CreateRankWeightedBag(specialDatas);
            for (int i = 0; i < specialDatas.Count; i++)
            {
                _specialAbilityDatas.Add(specialDatas[i].UID, specialDatas[i]);
            }
        }
        private ShuffleBag<AbilityDataBase> CreateRankWeightedBag(List<AbilityDataBase> datas)
        {
            Dictionary<EAbilityRank, int> rankCounts = CountRanks(datas);

            return new ShuffleBag<AbilityDataBase>(datas, data =>
            {
                if (data == null || !rankCounts.TryGetValue(data.Rank, out int count) || count <= 0)
                    return 0;

                float rankWeight = GameManager.Config.Ability.GetRankWeight(data.Rank);
                return rankWeight / count;
            });
        }
        private Dictionary<EAbilityRank, int> CountRanks(List<AbilityDataBase> datas)
        {
            Dictionary<EAbilityRank, int> rankCounts = new Dictionary<EAbilityRank, int>();

            for (int i = 0; i < datas.Count; i++)
            {
                EAbilityRank rank = datas[i].Rank;

                if (!rankCounts.ContainsKey(rank))
                    rankCounts[rank] = 0;

                rankCounts[rank]++;
            }

            return rankCounts;
        }
        public void GrantAbilityPoint(int currentLevel)
        {
            if (IsRuleAbilityLevel(currentLevel))
            {
                GainRuleAbility();
            }
            else
            {
                GainStatAbility();
            }

            GameTriggerEventBus.Instance.EnqueueInstanceTriggerEvent(EGameEventTrigger.OnTurnEnd, ProgressAbilitySelect);
        }

        public void GainRuleAbility()
        {
            var ruleSession = new AbilitySelectSession(GetRandomRuleAbilities(3), SelectRuleAbility, () => GetRandomRuleAbilities(3), _rerollCount);
            abilitySelectSessions.Enqueue(ruleSession);
        }
        public void GainStatAbility()
        {
            var statSession = new AbilitySelectSession(GetRandomStatAbilites(3), SelectStatAbility, () => GetRandomStatAbilites(3), _rerollCount);
            abilitySelectSessions.Enqueue(statSession);
        }

        private bool IsRuleAbilityLevel(int currentLevel)
        {
            int ruleAbilityLevel = GameManager.Config.Ability.RuleAbilityLevel;

            int achive = currentLevel % ruleAbilityLevel;

            return achive == 0;
        }
        public void ProgressAbilitySelect()
        {
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

        public void SelectRuleAbility(AbilityDataBase _abilityData)
        {
            IAbility ability = _playerAbilityFactory.CreateAbility(_abilityData);

            OnAddedRuleAbility?.Invoke(_abilityData);
            OnSelectAbility?.Invoke(ability);
        }
        public void SelectStatAbility(AbilityDataBase _abilityData)
        {
            Debug.Log($"{_abilityData.Name} / {_abilityData.Description}");

            IAbility ability = _playerAbilityFactory.CreateAbility(_abilityData);

            OnSelectAbility?.Invoke(ability);
        }


        public AbilityDataBase[] GetRandomRuleAbilities(int count)
        {
            AbilityDataBase[] abilityDatas = new AbilityDataBase[count];
            for (int i = 0; i < count; i++)
            {
                abilityDatas[i] = _specialAbilityBag.GetItem();
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
                SelectRuleAbility(abilities[i]);
            }
        }
        public void ObtainRandomStatAbility(int count)
        {
            var abilities = GetRandomStatAbilites(count);

            for (int i = 0; i < count; i++)
            {
                SelectStatAbility(abilities[i]);
            }
        }
        public void AddReroll(int amount = 1)
        {
            _rerollCount += amount;
        }
    }
}
