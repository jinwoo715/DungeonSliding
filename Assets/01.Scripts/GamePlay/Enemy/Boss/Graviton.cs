using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Graviton : StoneStatueEnemy, IBossAbility
    {
        HeavyGravityAbility _heavyGravityAbility;
        public void SetAbilityGetter(IEnemyAbilityGetter bossAbilityGetter)
        {
            //_heavyGravityAbility = new HeavyGravityAbility(bossAbilityGetter, this);
            StartCoroutine(_heavyGravityAbility.Excute());
        }
        public override void StartAttackAnimation()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
    }
}
