using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IPlayerAbilityFactory : IAbilityFactory
    {
        public IAbility CreateAbility(AbilityDataBase data);
    }
    public class PlayerAbilityFactory : IPlayerAbilityFactory
    {
        private IAbilityContextService _context;

        public void SetContext(IAbilityContextService context)
        {
            _context = context;
        }

        public IAbility CreateAbility(AbilityDataBase data)
        {
            if (data is RuleStatAbilityData ruleStatAbility)
                return CreateRuleStatAbility(ruleStatAbility);

            if (data is StatAbilityData statAbility)
                return CreateStatAbility(statAbility);

            if (data is RuleAbilityData ruleAbility)
                return CreateRuleAbility(ruleAbility);

            Debug.LogError($"Abiilty Type Error : {data?.GetType().Name}");
            return null;
        }
        private IAbility CreateStatAbility(StatAbilityData data)
        {
            return new StatAbility(data, _context);
        }
        private IAbility CreateRuleStatAbility(RuleStatAbilityData data)
        {
            return new RuleStatAbility(data, _context);
        }
        private IAbility CreateRuleAbility(RuleAbilityData data)
        {
            string abilityName = $"JW.DungeonSliding.GamePlay.Ability.{data.AbilityName}";

            Type type = Type.GetType(abilityName);

            if (type != null)
            {
                // 생성자 호출 (매개변수가 있는 경우 포함)
                object[] args = new object[] { data, _context };

                return (IAbility)Activator.CreateInstance(type, args);
            }
            else
            {
                Debug.LogError("Not Exist Ability");
                return null;
            }
        }
    }
}