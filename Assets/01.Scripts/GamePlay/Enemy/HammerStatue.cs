using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class HammerStatue : StoneStatueEnemy
    {
        public override void Attack(ICombatant target)
        {
            base.Attack(target);
            _animatorController.SetAnimationTrigger(ConstString.TWO_HAND_ATTACK_ANIM);
        }

        protected override DamageInfo CreateDamageInfo()
        {
            DamageInfo info = base.CreateDamageInfo();
            info.StatusEffect = EStatusEffectType.KnockBack;
            info.StatusAmount = 1;

            return info;
        }
    }
}