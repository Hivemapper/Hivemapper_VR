using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightControl : MonoBehaviour
{
    //Controls the Flight script on LeftHand and RightHand in Player.

    [Tooltip("Set flight speed.")]
    public float flightSpeed = 1f;

    [Tooltip("Enable rotation.")]
    public bool rotationEnabled = true;

    [Tooltip("Rotation speed.")]
    public float rotateSpeed = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
