using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
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
            Type type = Type.GetType(data.AbilityName.ToString());

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

            //switch (data.RuleType)
            //{
            //    case ERuleAbilityType.Revive:               return new ReviveAbility(data, context);               
            //    case ERuleAbilityType.Barrier:              return new BarrierAbility(data, context);              
            //    case ERuleAbilityType.WallBounce:           return new WallBounceAbility(data, context);           
            //    case ERuleAbilityType.SurroundEnemy:        return new SurroundEmpowerAbility(data, context);      
            //    case ERuleAbilityType.DoubleAttack:         return new ExtraAttackChance(data, context);           
            //    case ERuleAbilityType.CounterAttack:        return new CounterAttackAbility(data, context);        
            //    case ERuleAbilityType.DistanceDamageBonus:  return new SlideTileDamageBounsAbility(data, context); 
            //    case ERuleAbilityType.EnemyBind:            return new BindEnemyAbility(data, context);                    
            //    case ERuleAbilityType.Berserker:            return new DoubleEdgedAbility(data, context);          
            //    case ERuleAbilityType.ConvertHPToMoveCount: return new ConvertHpToMoveCount(data, context);        
            //    case ERuleAbilityType.ConvertMoveCountToHp: return new ConvertMoveCountToHp(data, context);        
            //    case ERuleAbilityType.RerollPlus:           return new RerollPlusAbility(data, context);           
            //    default:
            //        Debug.LogError($"Rule Ability Type Error {data.RuleType}");
            //        return null;
            //}
        }
    }
}