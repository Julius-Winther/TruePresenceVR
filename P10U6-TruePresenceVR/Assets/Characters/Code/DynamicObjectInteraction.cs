using UnityEngine;
using Convai.Scripts.Runtime.Features;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class DynamicObjectInteraction : MonoBehaviour
{
    private DynamicInfoController _dynamicInfoController;
    [SerializeField] private string objectName;
    private void Awake()
    {
        _dynamicInfoController = FindFirstObjectByType<DynamicInfoController>();
    }
    public void OnGrab()
    {
        _dynamicInfoController.SetDynamicInfo("User/Player is currently holding " + objectName);
    }
    public void OnDrop()
    {
        _dynamicInfoController.SetDynamicInfo("User/Player droppped " + objectName + " and is no longer holding anything");
    }
}
