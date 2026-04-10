using System;
using UnityEngine;

public class LifeCycleTest : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("Awake");
    }
    
    private void OnEnable()
    {
        Debug.Log("On Enable");
    }
    
    private void Start()
    {
        Debug.Log("Start");
    }

    private void OnDisable()
    {
        Debug.Log("On Disable");
    }

    private void OnDestroy()
    {
        Debug.Log("On Destroy");
    }
}
