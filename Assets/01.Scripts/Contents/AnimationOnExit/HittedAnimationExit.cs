using UnityEngine;

public class HittedAnimationExit : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var c = animator.GetComponent<JW.SlidingPuzzle.Creture>();
        if (c != null) c.EndHittedAnimation();
    }
}
