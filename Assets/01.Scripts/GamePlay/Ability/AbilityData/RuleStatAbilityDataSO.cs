using JW.DungeonSliding.GamePlay.Combat;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [UnityEngine.CreateAssetMenu(fileName = "RuleStatAbility", menuName = "Ability/Rule Stat Ability", order = 1)]
    public class RuleStatAbilityDataSO : StatAbilityDataSO
    {
        public EGameEventTrigger GameTriggerType = EGameEventTrigger.None;
        public ECreatureTrigger CreatureTriggerType = ECreatureTrigger.None;

        public EAbilityApplyStatType StatType = EAbilityApplyStatType.None;

        public ENextAttackType NextAttackType = ENextAttackType.None;
        public float NextAttackValue = 0;

        public int NeedStackCount = 0;

        public bool IsResetEnabled = false;
        public EGameEventTrigger ResetGameTrigger = EGameEventTrigger.None;
        public ECreatureTrigger ResetCreatureTrigger = ECreatureTrigger.None;
        public int ResetThreshold = 0;

        public override AbilityDataBase ToRuntimeData()
        {
            var data = new RuleStatAbilityData
            {
                PlayerStatType = PlayerStatType,
                ApplyType = ApplyType,
                RatioType = RatioType,
                StatValue = StatValue,
                GameTriggerType = GameTriggerType,
                CreatureTriggerType = CreatureTriggerType,
                StatType = StatType,
                NextAttackType = NextAttackType,
                NextAttackValue = NextAttackValue,
                NeedStackCount = NeedStackCount,
                IsResetEnabled = IsResetEnabled,
                ResetGameTrigger = ResetGameTrigger,
                ResetCreatureTrigger = ResetCreatureTrigger,
                ResetThreshold = ResetThreshold
            };

            CopyBaseDataTo(data);
            return data;
        }
    }
}
