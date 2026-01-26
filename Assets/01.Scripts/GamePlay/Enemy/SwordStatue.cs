using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class SwordStatue : StoneStatueEnemy
    {
        public override void Attack(ICombatant target)
        {
            base.Attack(target);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
    }
}
