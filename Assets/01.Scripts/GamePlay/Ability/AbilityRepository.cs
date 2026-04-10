using System.Collections.Generic;
using JW.Utility;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilityRepository
    {
        private readonly Dictionary<AbilityType, ShuffleBag<AbilityDataBase>> _bags = new();
        private readonly Dictionary<string, AbilityDataBase> _allAbilityDatas = new();

        public void Initialize(List<AbilityDataBase> statAbilities, List<AbilityDataBase> ruleAbilities)
        {
            _bags[AbilityType.Stat] = new ShuffleBag<AbilityDataBase>(new List<AbilityDataBase>(statAbilities));
            _bags[AbilityType.Rule] = new ShuffleBag<AbilityDataBase>(new List<AbilityDataBase>(ruleAbilities));

            foreach (var data in statAbilities) _allAbilityDatas[data.UID] = data;
            foreach (var data in ruleAbilities) _allAbilityDatas[data.UID] = data;
        }

        public AbilityDataBase[] GetRandomAbilities(AbilityType type, int count)
        {
            if (!_bags.TryGetValue(type, out var bag)) return null;

            AbilityDataBase[] abilityDatas = new AbilityDataBase[count];
            for (int i = 0; i < count; i++)
            {
                abilityDatas[i] = bag.GetItem();
            }
            return abilityDatas;
        }

        public AbilityDataBase GetAbilityData(string uid)
        {
            _allAbilityDatas.TryGetValue(uid, out var data);
            return data;
        }
    }
}
