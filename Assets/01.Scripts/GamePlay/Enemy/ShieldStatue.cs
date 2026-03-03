using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class ShieldStatue : StoneStatueEnemy
    {
        public void ExcuteAttack()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }

       

    }
}