using JW.DungeonSliding;
using JW.DungeonSliding.Core;
using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.GamePlay.Ability;
using JW.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
   

    public interface IAbilitySelectService
    {
        public void SetAbilitySession(AbilitySession session);
    }

    public class AbilitySession
    {
        public Func<AbilityData[]> GetRerollAbilityEvent;
        public Action<int> SelectAbiltyUIDEvent;
        public AbilityData[] Abilities;
        public int RerollCount;

        public AbilitySession(AbilityData[] abilities, Action<int> selectEvent, Func<AbilityData[]> rerollEvent, int rerollCount)
        {
            Abilities = abilities;
            SelectAbiltyUIDEvent = selectEvent;
            GetRerollAbilityEvent = rerollEvent;
            RerollCount = rerollCount;
        }

        public bool TryRerollAbilities()
        {
            if (RerollCount > 0)
            {
                Abilities = GetRerollAbilityEvent?.Invoke();
                RerollCount--;

                return true;
            }
            else return false;
        }
    }

    public class PlayerAbilitySystem
    {
        public Dictionary<EPlayerStat, IAbility> StatAbilityDic = new Dictionary<EPlayerStat, IAbility>();
        public Dictionary<ERuleEffect, IAbility> RuleAbilityDic = new Dictionary<ERuleEffect, IAbility>();

        ShuffleBag<AbilityData> _abilityBag;

        public List<IAbility> _hasAbilities;

        public AbilityFactory _abilityFactory = new AbilityFactory();

        private IGameModeChanger _gameModeChanger;
        private IAbilitySelectService _selectServeice;

        private int _maxRerollCount = 1;

        public void Init()
        {
            _abilityBag = new ShuffleBag<AbilityData>(GameManager.Instance.Resource.AllAbility);

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
        public void Wire(IAbilitySelectService abilitySelectService, IGameModeChanger gameModeChanger)
        {
            _selectServeice = abilitySelectService;
            _gameModeChanger = gameModeChanger;
        }
        public void GrantAbility(int _abilityUID)
        {
            Debug.Log("어빌리티 게또!");
            _gameModeChanger.ExitGameMode(EGameModeType.AbilityUI);
        }
        public AbilityData[] GetAbilityOptions()
        {
            throw new System.NotImplementedException();
        }
    }
}