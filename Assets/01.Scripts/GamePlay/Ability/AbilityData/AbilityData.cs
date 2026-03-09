using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "Ability", menuName = "Create Ability", order = 0)]
    public class AbilityData : ScriptableObject
    {
        [Header("Default Ability Data")]
        public int AbilityUID;
        public string Name;
        public string Description;
        public Sprite AbilitySprite;
        public EAbilityRank AbilityRank;
        public EAbilityEffectKind EAbilityEffectType;

        public List<EGameEventTrigger> AbilityTriggerTypes;

        public virtual List<EGameEventTrigger> GetEnrollTriggers => AbilityTriggerTypes;
    }
}
