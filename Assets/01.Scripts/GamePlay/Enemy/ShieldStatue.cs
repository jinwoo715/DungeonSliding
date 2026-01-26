using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class ShieldStatue : StoneStatueEnemy
    {
        public override void Attack(ICombatant target)
        {
            base.Attack(target);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }

        protected override DamageInfo CalculateRealAppliedDamage(DamageInfo damageInfo)
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