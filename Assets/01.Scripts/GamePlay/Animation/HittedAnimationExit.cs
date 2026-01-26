using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding.Presentation.Animation
{
    public class HittedAnimationExit : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var c = animator.GetComponent<Creature>();
            if (c != null) c.EndHittedAnimation();
        }
    }
}