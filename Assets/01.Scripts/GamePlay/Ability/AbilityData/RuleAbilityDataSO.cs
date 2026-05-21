using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "RuleAbility", menuName = "Ability/Rule Ability", order = 2)]
    public class RuleAbilityDataSO : AbilityDataSOBase
    {
        public EGameEventTrigger GameTrigger = EGameEventTrigger.None;
        public ECreatureTrigger CreatureTrigger = ECreatureTrigger.None;
        public string AbilityName;
        public float P1;
        public float P2;
        public string Notes;

        public override AbilityDataBase ToRuntimeData()
        {
            var data = new RuleAbilityData
            {
                GameTrigger = GameTrigger,
                CreatureTrigger = CreatureTrigger,
                AbilityName = AbilityName,
                P1 = P1,
                P2 = P2,
                Notes = Notes
            };

            CopyBaseDataTo(data);
            return data;
        }
    }
}
