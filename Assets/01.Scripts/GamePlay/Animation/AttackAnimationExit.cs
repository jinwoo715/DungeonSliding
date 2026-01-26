using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.Presentation.Animation
{
    public class AttackAnimationExit : StateMachineBehaviour
    {
        private Creature _creture;
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (_creture == null)
                _creture = animator.GetComponentInParent<Creature>();

            if (_creture != null)
                _creture.EndAttackAnimation();

            else
                Debug.LogError("Not Exist Creture");

        }
    }
}