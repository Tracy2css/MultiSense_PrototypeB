using UnityEngine;

public class AnimationLoopControl : MonoBehaviour
{
    public Animator animator;
    public string loopParameterName = "loopCount";  // Parameter name in Animator
    private int currentLoop = 0;

    void Update()
    {
        // Get the current animation state information
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Check if Sitting_leg animation is playing
        if (stateInfo.IsName("Sitting_leg"))
        {
            // Get the current loop count by rounding down normalizedTime
            int newLoop = Mathf.FloorToInt(stateInfo.normalizedTime);

            // Get the current loop count from Animator
            int loopCount = animator.GetInteger(loopParameterName);

            // If normalizedTime exceeded the current loop count, increment the parameter
            if (newLoop > currentLoop)
            {
                currentLoop = newLoop;
                loopCount++;  // Increment the loop count
                animator.SetInteger(loopParameterName, loopCount);  // Update the parameter in Animator
            }
        }
        // Check if the animation is in Rubbing Arm state
        else if (stateInfo.IsName("Typing"))
        {
            // Reset loop count when in Rubbing Arm state
            animator.SetInteger(loopParameterName, 0);
            currentLoop = 0;
        }
    }
}
