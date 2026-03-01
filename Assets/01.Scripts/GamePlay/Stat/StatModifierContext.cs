using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public struct StatModifierContext
    {
        public ECreatureStatType TargetStat;      // 적용할 타입 (결과적으로 오르는 스탯. 예: 공격력)
        public StatModifyType ModifyType;         // 방식 (Add, Multiple, Ratio)
        public float Value;                       // 수치 (10, 0.1f 등)
        public ECreatureStatType RatioBaseStat;   // 비율을 계산할 기준 타입 (예: 최대 체력)

        // 1. 일반적인 Add, Multiple을 쓸 때 부르는 생성자 (기준 타입은 필요 없음)
        public StatModifierContext(ECreatureStatType target, StatModifyType type, float value)
        {
            TargetStat = target;
            ModifyType = type;
            Value = value;
            RatioBaseStat = target; // 안 쓰는 값이지만 초기화
        }

        // 2. Ratio(비례) 전용으로 부르는 생성자
        public StatModifierContext(ECreatureStatType target, float value, ECreatureStatType ratioBase)
        {
            TargetStat = target;
            ModifyType = StatModifyType.Ratio;
            Value = value;
            RatioBaseStat = ratioBase;
        }
    }
}
