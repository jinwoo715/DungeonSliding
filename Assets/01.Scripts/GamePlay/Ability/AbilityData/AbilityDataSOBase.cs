using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public abstract class AbilityDataSOBase : ScriptableObject
    {
        public string UID;
        public string Name;
        public string Description;
        public string IconName;
        public EAbilityRank Rank;

        public abstract AbilityDataBase ToRuntimeData();

        protected void CopyBaseDataTo(AbilityDataBase data)
        {
            data.UID = UID;
            data.Name = Name;
            data.Description = Description;
            data.IconName = IconName;
            data.Rank = Rank;
        }
    }
}
