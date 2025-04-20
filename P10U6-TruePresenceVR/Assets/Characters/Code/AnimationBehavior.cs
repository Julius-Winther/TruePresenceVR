using UnityEngine;

public class AnimationBehavior : StateMachineBehaviour
{
    [SerializeField] private int _numberOfIdleAnimations;
    [SerializeField] private float _timeThreshold;    
    [SerializeField] private float _AnimationTransitionTime;
    [SerializeField] private bool isIdle = true;
    private EmotionHandler emotionHandler;
    private bool hasChangedAnimations;
    private float AnimationTime;
    private float Animation;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        emotionHandler = animator.GetComponent<EmotionHandler>();
        //Animation = 0;
        ResetEverything();
    } 

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        /*if(hasChangedAnimations == false)
        {
            AnimationTime += Time.deltaTime;
            //if(AnimationTime > _timeThreshold && stateInfo.normalizedTime % 1 < 0.02f)
            if(AnimationTime > _timeThreshold)
            {
                hasChangedAnimations = true;
                Animation = Random.Range(0,_numberOfIdleAnimations+1);
                //Animation = emotionHandler.EmotinalState_Blend;
            }
        }*/
        /*if(stateInfo.normalizedTime % 1 > 0.98f)
        {
            ResetEverything();
        }*/
        //animator.SetFloat("IdleBlend", 1, _AnimationTransitionTime, Time.deltaTime);    


        AnimationTime += Time.deltaTime;
        if(hasChangedAnimations == false)
        {
            hasChangedAnimations = true;
            //Animation = Random.Range(0,_numberOfIdleAnimations+1);
            Animation = emotionHandler.EmotinalState_Blend + 0.05f;
            
        }
        if(AnimationTime > _timeThreshold)
        {
            ResetEverything();
        } 


        if(isIdle)
        {
            animator.SetFloat("IdleBlend", Animation, _AnimationTransitionTime, Time.deltaTime);
            Debug.Log("Idle Anim State: " + (Animation - 0.05f));
        }
        else
        {
            animator.SetFloat("TalkBlend", Animation, _AnimationTransitionTime, Time.deltaTime);
            Debug.Log("Talk Anim State: " + (Animation - 0.05f));
        }

    }

    private void ResetEverything()
    {
        hasChangedAnimations = false;
        AnimationTime = 0;
        Animation = 0;
        //animator.SetFloat("IdleBlend", 0);
    }
}
