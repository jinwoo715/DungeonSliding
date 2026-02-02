using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class SwordStatue : StoneStatueEnemy
    {
        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
    }
}
