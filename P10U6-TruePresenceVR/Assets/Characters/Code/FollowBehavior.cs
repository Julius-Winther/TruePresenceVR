using UnityEngine;

public class FollowBehavior : MonoBehaviour
{
    [SerializeField] private Transform _convaiNPC_Transform;
    [SerializeField] private Transform _player_Transform;
    [SerializeField] private GameObject _convaiNarrativeTrigger;
    [SerializeField] private float _distanceThreshold = 5;
    [SerializeField] private float _Timer = 5;
    [SerializeField] private bool Debug_Distance = false;
    [SerializeField] private bool Debug_Timer = false;
    private float distanceToPlayer;
    private float elapsedTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _convaiNarrativeTrigger.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(_convaiNPC_Transform.position, _player_Transform.position);
        elapsedTime += Time.deltaTime;
        
        // Debug
        if(Debug_Distance)
        {
        Debug.Log("Distance To player From NPC" + distanceToPlayer);
        }
        if(Debug_Timer)
        {
        Debug.Log("Elapsed Time" + elapsedTime);
        }
        //
        
        if(Mathf.Abs(distanceToPlayer) > Mathf.Abs(_distanceThreshold) && elapsedTime >_Timer)
        {
            _convaiNarrativeTrigger.SetActive(true);
            elapsedTime = 0;
        }
        else if(Mathf.Abs(distanceToPlayer) < Mathf.Abs(_distanceThreshold))
        {
            _convaiNarrativeTrigger.SetActive(false);
        }
    }
}
