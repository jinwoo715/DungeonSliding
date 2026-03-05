using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilityFactory
    {
        public IAbilityBase CreateAbility(AbilityDataBase data, AbilityHost host)
        {
            string abilityType = data.UID.Substring(0, 2);

            if (abilityType == "SA")
            {
                StatAbilityData statAbility = data as StatAbilityData;
                return CreateStatAbility(data as StatAbilityData, host);
            }
            else if(abilityType == "RA")
            {
                return CreateRuleAbility(data as RuleAbilityData, host);
            }
            else
            {
                Debug.LogError("Abiilty Type Error");
                return null;
            }
        }

        public IAbilityBase CreateStatAbility(StatAbilityData data, AbilityHost host)
        {
            return new StatAbility(data, host);
        }

        public IAbilityBase CreateRuleAbility(RuleAbilityData data, AbilityHost host)
        {
            switch (data.RuleType)
            {
                case ERuleAbilityType.Revive:               return new ReviveAbility(data, host);               //し
                case ERuleAbilityType.Barrier:              return new BarrierAbility(data, host);              //し
                case ERuleAbilityType.WallBounce:           return new WallBounceAbility(data, host);           //し
                case ERuleAbilityType.SurroundEnemy:        return new SurroundEmpowerAbility(data, host);      //し
                case ERuleAbilityType.DoubleAttack:         return new ExtraAttackChance(data, host);           //し
                case ERuleAbilityType.CounterAttack:        return new CounterAttackAbility(data, host);        //し
                case ERuleAbilityType.DistanceDamageBonus:  return new SlideTileDamageBounsAbility(data, host); //し
                case ERuleAbilityType.EnemyBind:            return new BindEnemyAbility(data, host);            //し        
                case ERuleAbilityType.Berserker:            return new DoubleEdgedAbility(data, host);          //し
                case ERuleAbilityType.ConvertHPToMoveCount: return new ConvertHpToMoveCount(data, host);        //し
                case ERuleAbilityType.ConvertMoveCountToHp: return new ConvertMoveCountToHp(data, host);        //し
                case ERuleAbilityType.RerollPlus:           return new RerollPlusAbility(data, host);           //し
                default:
                    Debug.LogError($"Rule Ability Type Error {data.RuleType}");
                    return null;
            }
        }
        
    }
}