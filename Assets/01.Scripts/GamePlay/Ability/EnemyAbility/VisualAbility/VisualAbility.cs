using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Enemy
{
    public class Blind : EnemyAbilityBase
    {
        IVisualController _visualController;
        bool isBlined = false;

        int _turnCount = 0;
        int _blindCount = 0;

        public Blind(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (isBlined == true)
            {
                _blindCount++;

                if(_blindCount >= P2)
                {
                    isBlined = false;
                    _blindCount = 0;
                    _visualController.ExitBlind();
                }
            }
            else
            {
                _turnCount++;
                if (_turnCount >= P1)
                {
                    isBlined = true;
                    _turnCount = 0;
                    _visualController.EnterBlind();
                }
            }
            yield return null;
        }

        protected override void BindService()
        {
            BindService(ref _visualController);
        }
    }
}
