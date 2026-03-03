using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class HammerStatue : StoneStatueEnemy
    {
        public void ExcuteAttack()
        {
            _animatorController.SetAnimationTrigger(ConstString.TWO_HAND_ATTACK_ANIM);
        }
    }
}