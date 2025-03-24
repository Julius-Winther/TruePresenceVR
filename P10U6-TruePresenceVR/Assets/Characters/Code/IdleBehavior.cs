using UnityEngine;

public class IdleBehavior : StateMachineBehaviour
{

    [SerializeField] private int _numberOfIdleAnimations;
    [SerializeField] private float _timeThreshold;    
    [SerializeField] private float _AnimationTransitionTime;
    private bool hasChangedIdle;
    private float IdleTime;
    private int idleAnimation;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        ResetEverything();
    } 

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerindex)
    {
        if(hasChangedIdle == false)
        {
            IdleTime += Time.deltaTime;
            if(IdleTime > _timeThreshold && stateInfo.normalizedTime % 1 < 0.02f)
            {
                hasChangedIdle = true;
                idleAnimation = Random.Range(0, _numberOfIdleAnimations + 1);
            }
        }
        else if (stateInfo.normalizedTime % 1 > 0.98f)
        {
            ResetEverything();
        }

        animator.SetFloat("IdleBlend", idleAnimation, _AnimationTransitionTime, Time.deltaTime);
    }

    private void ResetEverything()
    {
        hasChangedIdle = false;
        IdleTime = 0;
        idleAnimation = 0;
        //animator.SetFloat("IdleBlend", 0);
    }


}
