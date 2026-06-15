using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "AbilityDatabase", menuName = "Ability/Ability Database", order = 3)]
    public class AbilityDatabaseSO : ScriptableObject
    {
        public List<StatAbilityDataSO> StatAbilities = new();
        public List<RuleStatAbilityDataSO> RuleStatAbilities = new();
        public List<RuleAbilityDataSO> RuleAbilities = new();

        public bool HasAnyAbilityData =>
            StatAbilities.Count > 0 || RuleStatAbilities.Count > 0 || RuleAbilities.Count > 0;

        public List<AbilityDataBase> CreateStatAbilityRuntimeData()
        {
            var datas = new List<AbilityDataBase>(StatAbilities.Count);

            for (int i = 0; i < StatAbilities.Count; i++)
            {
                if (StatAbilities[i] != null)
                    datas.Add(StatAbilities[i].ToRuntimeData());
            }

            return datas;
        }

        public List<AbilityDataBase> CreateRuleStatAbilityRuntimeData()
        {
            var datas = new List<AbilityDataBase>(RuleStatAbilities.Count);

            for (int i = 0; i < RuleStatAbilities.Count; i++)
            {
                if (RuleStatAbilities[i] != null)
                    datas.Add(RuleStatAbilities[i].ToRuntimeData());
            }

            return datas;
        }

        public List<AbilityDataBase> CreateRuleAbilityRuntimeData()
        {
            var datas = new List<AbilityDataBase>(RuleStatAbilities.Count + RuleAbilities.Count);

            for (int i = 0; i < RuleStatAbilities.Count; i++)
            {
                if (RuleStatAbilities[i] != null)
                    datas.Add(RuleStatAbilities[i].ToRuntimeData());
            }

            for (int i = 0; i < RuleAbilities.Count; i++)
            {
                if (RuleAbilities[i] != null)
                    datas.Add(RuleAbilities[i].ToRuntimeData());
            }

            return datas;
        }

        public List<AbilityDataBase> CreatePureRuleAbilityRuntimeData()
        {
            var datas = new List<AbilityDataBase>(RuleAbilities.Count);

            for (int i = 0; i < RuleAbilities.Count; i++)
            {
                if (RuleAbilities[i] != null)
                    datas.Add(RuleAbilities[i].ToRuntimeData());
            }

            return datas;
        }
    }
}
