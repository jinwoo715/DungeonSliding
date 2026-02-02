using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class HammerStatue : StoneStatueEnemy
    {
        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.TWO_HAND_ATTACK_ANIM);
        }

        protected override DamageContext CreateDamageContext()
        {
            DamageContext info = base.CreateDamageContext();
            info.StatusEffect = EStatusEffectType.KnockBack;
            info.StatusAmount = 1;

            return info;
        }
    }
}