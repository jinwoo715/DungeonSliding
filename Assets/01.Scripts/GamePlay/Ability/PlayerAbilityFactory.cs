using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class PlayerAbilityFactory : IAbilityFactory
    {
        private IAbilityContextService _context;

        public void SetContext(IAbilityContextService context)
        {
            _context = context;
        }

        public IAbility CreateAbility(AbilityDataBase data)
        {
            string abilityType = data.UID.Substring(0, 2);

            if (abilityType == "SA")
            {
                StatAbilityData statAbility = data as StatAbilityData;
                return CreateStatAbility(data as StatAbilityData);
            }
            else if(abilityType == "RA")
            {
                return CreateRuleAbility(data as RuleAbilityData);
            }
            else
            {
                Debug.LogError("Abiilty Type Error");
                return null;
            }
        }
        private IAbility CreateStatAbility(StatAbilityData data)
        {
            return new StatAbility(data, _context);
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