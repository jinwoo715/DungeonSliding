using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class ShieldStatue : StoneStatueEnemy
    {
        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }

        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            if (damageInfo.Attacker.Direction == ReverseDirection(Direction))
            {
                damageInfo.Damage = 0;
                return damageInfo;
            }
            else
            {
                return base.CalculateRealAppliedDamage(damageInfo);
            }
        }

    }
}