using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Doppelganger : StoneStatueEnemy, IBossAbility
    {
        private CopyAbility _copyAbility;

        public void SetAbilityGetter(IEnemyAbilityGetter bossAbilityGetter)
        {
            //_copyAbility = new CopyAbility(bossAbilityGetter, this);
        }

        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
    }
}
