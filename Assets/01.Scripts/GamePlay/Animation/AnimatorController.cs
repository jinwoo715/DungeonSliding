using System;
using UnityEngine;
namespace JW.DungeonSliding
{
    public class AnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;

        public Action OnHitTimeingEvent;
        public Action OnEndAttackAnimationEvent;
        public Action OnEndHittedAnimationEvent;

        public void OnTriggerAttackHit()
        {
            OnHitTimeingEvent?.Invoke();
        }
        public void OnTriggerEndAttackAnimation()
        {
            OnEndAttackAnimationEvent?.Invoke();
        }
        public void OnTriggerEndHittedAnimation()
        {
            OnEndHittedAnimationEvent?.Invoke();
        }

        public void SetAnimationTrigger(string triggerKey)
        {
            _animator.SetTrigger(triggerKey);
        }
        public void SetInt(string key, int value)
        {
            _animator.SetInteger(key, value);
        }
    }
}
