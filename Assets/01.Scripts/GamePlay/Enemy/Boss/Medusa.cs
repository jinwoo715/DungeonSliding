using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Medusa : StoneStatueEnemy, IBossAbility
    {
        FacingMoveBanAbility _facingMoveBanAbility;

        public void OnEnable()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, ExcuteAbility);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, ExcuteAbility);
        }

        public void SetAbilityGetter(IEnemyAbilityGetter bossAbilityGetter)
        {
            //_facingMoveBanAbility = new FacingMoveBanAbility(bossAbilityGetter, this);
        }

        public void ExcuteAbility()
        {
            StartCoroutine(_facingMoveBanAbility.Execute());
        }
        public void ExcuteAttack()
        {
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
    }
}
