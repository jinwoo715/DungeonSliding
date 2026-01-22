using JW.SlidingPuzzle;
using UnityEngine;

public class AttackAnimationExit : StateMachineBehaviour
{
    private Creture _creture;
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(_creture == null)
            _creture = animator.GetComponentInParent<JW.SlidingPuzzle.Creture>();

        if (_creture != null)
            _creture.EndAttackAnimation();

        else
            Debug.LogError("Not Exist Creture");

    }
}
