using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IPlayerAbilityCreater
    {
        IAbility CreateAbility(string abilityType, IAbilityContextService context);
    }
    public interface IEnemyAbilityCreater
    {
        List<IAbility> CreateAbility(List<EnemyAbilityData> datas, ICombatant owner, int section);
    }

    public class AbilityFactory
    {
        public IAbility CreateAbility(AbilityDataBase data, IAbilityContextService context)
        {
            string abilityType = data.UID.Substring(0, 2);

            if (abilityType == "SA")
            {
                StatAbilityData statAbility = data as StatAbilityData;
                return CreateStatAbility(data as StatAbilityData, context);
            }
            else if(abilityType == "RA")
            {
                return CreateRuleAbility(data as RuleAbilityData, context);
            }
            else
            {
                Debug.LogError("Abiilty Type Error");
                return null;
            }
        }
        public IAbility CreateStatAbility(StatAbilityData data, IAbilityContextService context)
        {
            return new StatAbility(data, context);
        }
        public IAbility CreateRuleAbility(RuleAbilityData data, IAbilityContextService context)
        {
            string abilityName = $"JW.DungeonSliding.GamePlay.Ability.{data.AbilityName}";

            Type type = Type.GetType(abilityName);

            if (type != null)
            {
                // 생성자 호출 (매개변수가 있는 경우 포함)
                object[] args = new object[] { data, context };

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