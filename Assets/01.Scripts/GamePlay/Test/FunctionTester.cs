using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Ability;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class FunctionTester : MonoBehaviour
    {
        public PlayerAbilitySystem _abilitySystem;

        public List<AbilityDataBase> datas = GameManager.Data.Abilities;

        public int GetAbilityIndex;

        public void Init(PlayerAbilitySystem abilitySystem)
        {
            _abilitySystem = abilitySystem;
        }

        private IEnumerator Start()
        {
            yield return null;

            datas = GameManager.Data.Abilities;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.A))
            {
                _abilitySystem.SelectAbility(datas[GetAbilityIndex]);
            }
        }
    }
}
