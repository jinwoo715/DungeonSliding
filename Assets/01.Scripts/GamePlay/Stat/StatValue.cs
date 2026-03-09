using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public class StatValue
    {
        public int Base;      // ±âº»°ª
        public int Add;       // µ¡¼À º¸³Ê½º(+)
        public float Mul;     // °ö¼À º¸³Ê½º(¡¿), ±âº» 1f

        public Dictionary<ECreatureStatType, float> RatioValueByStat;
        public StatValue(int baseValue)
        {
            Base = baseValue;
            Add = 0;
            Mul = 1;

            RatioValueByStat = new();
        }

        public void ModifiyStat(StatModifierContext modifierContext)
        {
            switch (modifierContext.ModifyType)
            {
                case EApplyStatType.Add:
                    AddAdd(Mathf.RoundToInt(modifierContext.Value));
                    break;
                case EApplyStatType.Multiple:
                    AddMultiple(modifierContext.Value);
                    break;
                case EApplyStatType.Ratio:
                    AddRatio(modifierContext.RatioBaseStat, modifierContext.Value);
                    break;
            }
        }

        public void AddRatio(ECreatureStatType stat, float value)
        {
            if (!RatioValueByStat.ContainsKey(stat))
                RatioValueByStat.Add(stat, 0);

            RatioValueByStat[stat] += value;
        }

        private void AddAdd(int value)
        {
            Add += value;
        }

        private void AddMultiple(float multiple)
        {
            Mul += multiple;
        }

        public int Final(IStatReadOnly StatReadOnly)
        {
            float addRatioValue = 0;
            foreach (var item in RatioValueByStat)
            {
                addRatioValue += StatReadOnly.Get(item.Key) * item.Value;
            }

            return Mathf.FloorToInt((Base + Add + addRatioValue) * Mul);
        }
        public void Clear()
        {
            Add = 0;
            Mul = 1f;
            RatioValueByStat.Clear();
        }
    }
}
