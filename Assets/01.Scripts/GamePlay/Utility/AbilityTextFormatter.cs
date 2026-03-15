using JW.DungeonSliding.GamePlay.Ability;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JW.DungeonSliding
{
    public static class AbilityTextFormatter
    {
        public static string ConvertPlayerAbility(AbilityDataBase abilityData)
        {
            if (abilityData is StatAbilityData sa)
                return ConvertStatAbilityDescription(sa);

            if (abilityData is RuleAbilityData ra)
                return ConvertRuleAbilityDescription(ra);

            return string.Empty;
        }

        public static string ConvertStatAbilityDescription(StatAbilityData sa)
        {
            StringBuilder sb = new StringBuilder(sa.Description);
            var convertList = new Dictionary<string, string>();

            string statValue = sa.ApplyType == EApplyStatType.Add ? sa.StatValue.ToString() : (sa.StatValue * 100).ToString();
            convertList.Add("{StatValue}", statValue);

            string nextAttackValue = sa.NextAttackType == GamePlay.Combat.ENextAttackType.Multiple ? (sa.NextAttackValue * 100).ToString() : sa.NextAttackValue.ToString();
            convertList.Add("{NextAttackValue}", nextAttackValue);

            convertList.Add("{NeedStackCount}", sa.NeedStackCount.ToString());
            convertList.Add("{ResetThreshold}", sa.ResetThreshold.ToString());

            foreach (var replaceData in convertList)
            {
                sb.Replace(replaceData.Key, replaceData.Value);
            }

            return sb.ToString();
        }
        public static string ConvertRuleAbilityDescription(RuleAbilityData ra)
        {
            StringBuilder sb = new StringBuilder(ra.Description);
            var convertList = new Dictionary<string, string>();

            convertList.Add("{P1}", ra.P1.ToString());
            convertList.Add("{P2}", ra.P2.ToString());

            foreach (var replaceData in convertList)
            {
                sb.Replace(replaceData.Key, replaceData.Value);
            }

            return sb.ToString();
        }
        public static string ConvertEnemyAbilityDescription(StatAbilityData sa)
        {
            StringBuilder sb = new StringBuilder(sa.Description);

            return sb.ToString();
        }

    }
}
