using UnityEngine;

public class AnimationBehavior : StateMachineBehaviour
{
    [SerializeField] private int _numberOfIdleAnimations;
    [SerializeField] private float _timeThreshold;    
    [SerializeField] private float _AnimationTransitionTime;
    private EmotionHandler emotionHandler;
    private bool hasChangedAnimations;
    private float AnimationTime;
    private int Animation;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        emotionHandler = animator.GetComponent<EmotionHandler>();
        ResetEverything();
    } 

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        if(hasChangedAnimations == false)
        {
            AnimationTime += Time.deltaTime;
            if(AnimationTime > _timeThreshold && stateInfo.normalizedTime % 1 < 0.02f)
            {
                hasChangedAnimations = true;
                Animation = emotionHandler.EmotinalState_Blend;
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f)
        {
            ResetEverything();
        }

        animator.SetFloat("IdleBlend", Animation, _AnimationTransitionTime, Time.deltaTime);
    }

    private void ResetEverything()
    {
        hasChangedAnimations = false;
        AnimationTime = 0;
        Animation = 0;
        //animator.SetFloat("IdleBlend", 0);
    }
}
