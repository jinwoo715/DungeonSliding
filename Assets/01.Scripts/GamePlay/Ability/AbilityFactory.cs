using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilityFactory
    {
        public IAbility CreateAbility(AbilityData data, AbilityHost host)
        {
            IAbility createdAbility = null;

            switch (data.EAbilityEffectType)
            {
                case EAbilityEffectKind.Stat:

                    createdAbility = CreateStatAbility(data, host);

                    break;
                case EAbilityEffectKind.Rule:

                    CreateRuleAbility(data, host);

                    break;
            }

            return createdAbility;
        }

        public IAbility CreateStatAbility(AbilityData data, IAbilityHost entity)
        {
            IAbility createdAbility = null;

            if (data is InstantStatAbiltyData)
            {
                createdAbility = new InstantStatAbility(entity, (InstantStatAbiltyData)data);
            }
            else if (data is TriggerStatAbiltyData)
            {
                createdAbility = new TriggerStatAbilty(entity, (TriggerStatAbiltyData)data);
            }
            else if (data is StackableStatAbilityData)
            {
                createdAbility = new StackableAbility(entity, (StackableStatAbilityData)data);
            }

            return createdAbility;
        }
        public IAbility CreateRuleAbility(AbilityData data, IAbilityHost host)
        {
            RuleAbilitySOData ruleData = (RuleAbilitySOData)data;
            IAbility createdAbility = null;
            switch (ruleData.RuleAbilityType)
            {
                case ERuleAbilityType.Revive:
                    createdAbility = new ReviveAbility(ruleData, host);
                    break;
                case ERuleAbilityType.Barrier:
                    createdAbility = new BarrierAbility(ruleData, host);
                    break;
                case ERuleAbilityType.WallBounce:
                    createdAbility = new WallBounceAbility(ruleData, host);
                    break;
                case ERuleAbilityType.SurroundEnemy:
                    createdAbility = new SurroundEmpowerAbility(ruleData, host);

                    break;
                case ERuleAbilityType.DoubleAttack:
                    createdAbility = new ExtraAttackChance(ruleData, host);

                    break;
                case ERuleAbilityType.CounterAttack:
                    createdAbility = new CounterAttack(ruleData, host);

                    break;
                case ERuleAbilityType.DistanceDamageBonus:
                    createdAbility = new SlideAmplifierAbility(ruleData, host);

                    break;
                case ERuleAbilityType.EnemyBind:
                    createdAbility = new BindEnemyAbility(ruleData, host);

                    break;
                case ERuleAbilityType.Berserker:
                    createdAbility = new DoubleEdgedAbility(ruleData, host);

                    break;
                case ERuleAbilityType.LastResortMove:
                    createdAbility = new ConvertHpToMoveCount(ruleData, host);

                    break;
                case ERuleAbilityType.LastResortHP:
                    createdAbility = new ConvertMoveCountToHp(ruleData, host);

                    break;

                case ERuleAbilityType.RerollPlus:
                    createdAbility = new RerollPlusAbility(ruleData, host);

                    break;
            }

            return createdAbility;
        }
    }
}