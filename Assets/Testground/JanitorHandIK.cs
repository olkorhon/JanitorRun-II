using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JanitorHandIK : MonoBehaviour {
    protected Animator animator;

    public bool ikActive = false;
    public Transform leftHandObj = null;
    public Transform rightHandObj = null;

    public Transform lookObj = null;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void enableIK(AvatarIKGoal goal)
    {
        animator.SetIKPositionWeight(goal, 1);
        animator.SetIKRotationWeight(goal, 1);
    }

    private void disableIK(AvatarIKGoal goal)
    {
        animator.SetIKPositionWeight(goal, 0);
        animator.SetIKRotationWeight(goal, 0);
    }

    private void setPosAndRot(AvatarIKGoal goal, Transform target)
    {
        animator.SetIKPosition(goal, target.position);
        animator.SetIKRotation(goal, target.rotation);
    }

    // Callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                if (lookObj != null)
                {
                    // Set the look target position
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookObj.position);
                }

                if (leftHandObj != null)
                {
                    // Set the left hand target position and rotation
                    enableIK(AvatarIKGoal.LeftHand);
                    setPosAndRot(AvatarIKGoal.LeftHand, leftHandObj);
                }

                if (rightHandObj != null)
                {
                    // Set the right hand target position and rotation
                    enableIK(AvatarIKGoal.RightHand);
                    setPosAndRot(AvatarIKGoal.RightHand, rightHandObj);

                }

            }
            else
            {
                //if the IK is not active, set the position and rotation of the hand and head back to the original position
                disableIK(AvatarIKGoal.RightHand);
                disableIK(AvatarIKGoal.LeftHand);
                animator.SetLookAtWeight(0);
            }
        }
    }
}
