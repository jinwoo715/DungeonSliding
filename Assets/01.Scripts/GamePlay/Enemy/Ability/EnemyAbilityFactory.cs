using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Combat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class EnemyAbilityFactory : IEnemyAbilityCreater
    {
        IAbilityContextService _service;

        public void Init(IAbilityContextService service)
        {
            _service = service;
        }

        public List<IAbility> CreateAbility(List<EnemyAbilityData> datas, ICombatant owner, int section)
        {
            List<IAbility> abilityList = new List<IAbility>();

            for (int i = 0; i < datas.Count; i++)
            {
                abilityList.Add(CreateAbility(datas[i], owner, section));
            }

            return abilityList;
        }

        private IAbility CreateAbility(EnemyAbilityData data, ICombatant host, int section)
        {
            string abilityName = $"JW.DungeonSliding.GamePlay.Ability.Enemy.{data.AbilityType}";
            
            Type type = Type.GetType(abilityName);

            if (type != null)
            {
                // 생성자 호출 (매개변수가 있는 경우 포함)
                object[] args = new object[] { data, _service, host, section };

                return (IAbility)Activator.CreateInstance(type, args);
            }
            else
            {
                Debug.LogError($"Not Exist Ability {abilityName}");
                return null;
            }
        }
    }
}
