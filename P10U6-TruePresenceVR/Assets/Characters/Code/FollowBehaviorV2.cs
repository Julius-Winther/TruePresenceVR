using Convai.Scripts.Runtime.Features;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class FollowBehaviorV2 : MonoBehaviour
{
    [Tooltip("The transform of the target that the NPC needs to follow")] 
    [SerializeField] private Transform _player_Transform;
    
    [Tooltip("The distance between the NPC and the player reqiured to trigger the follow behavior")] 
    [SerializeField] private float _distanceThreshold = 5;

    [Tooltip("The cooldown between being able to trigger the follow behaviour even when the distance requirement is met")] 
    [SerializeField] private float _CoolDown = 5;

    [Tooltip("List of locations that the NPC should be aware of. It is important that the name of the loactions matches what the location is actually called")] 
    [SerializeField] private List<GameObject> LocationGameObjects = new List<GameObject>();

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private Collider TimerRestRangeHitbox;
    private DynamicInfoController _dynamicInfoController;
    private float distanceToPlayer;
    private float elapsedTime;
    private bool hasJustWalked = false;
    private float closestLocation = 100000; //Arbitrary high number
    private string closestLocationName = "Melab entrance";
    private float currentDistance = 0;

    // DEBUGING

    [Tooltip("Display Distance to the player from the NPC (in the console)")] 
    [SerializeField] private bool Debug_DistanceToPlayer = false;

    [Tooltip("Display the elapsed time of the follow behaviour cooldown (in the console)")] 
    [SerializeField] private bool Debug_CoolDown = false;

    [Tooltip("Display the name of the closest location to the NPC (in the console)")] 
    [SerializeField] private bool Debug_ClosestLocation = false;

    //

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        _dynamicInfoController = GetComponent<DynamicInfoController>();
    }


    void Update()
    {
        closestLocation = currentDistance;
        foreach(GameObject location in LocationGameObjects)
        {
            currentDistance = Vector3.Distance(this.transform.position, location.transform.position);


             if(Mathf.Abs(currentDistance) <= Mathf.Abs(closestLocation))
             {
                closestLocation = currentDistance;
                closestLocationName = location.name;
                _dynamicInfoController.SetDynamicInfo("Current location for Emilio and the player is " + closestLocationName + " and its " + closestLocation + "M away");
             }

        }



        distanceToPlayer = Vector3.Distance(this.transform.position, _player_Transform.position);
        elapsedTime += Time.deltaTime;
        
        // Debug
        if(Debug_DistanceToPlayer)
        {
            Debug.Log("Distance To player From NPC" + distanceToPlayer);
        }
        if(Debug_CoolDown)
        {
            Debug.Log("Elapsed Time" + elapsedTime);
        }
        if(Debug_ClosestLocation)
        {
            Debug.Log("Closest Location To Emilio is " + closestLocationName); //+ " and its " + closestLocation + "m away");
        }
        //

        if(Mathf.Abs(distanceToPlayer) > Mathf.Abs(_distanceThreshold) && elapsedTime >_CoolDown)
        {
            navMeshAgent.destination = _player_Transform.position;
            transform.LookAt(_player_Transform);
            animator.Play("Walking");
            elapsedTime = 0;
            hasJustWalked = true;
        }
        if(Mathf.Abs(distanceToPlayer) < 1.3 && hasJustWalked)
        {
            animator.Play("Idle");
            hasJustWalked = false;
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            elapsedTime = 0;
        }
    }
}

