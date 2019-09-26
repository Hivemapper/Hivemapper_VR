using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightControl : MonoBehaviour
{
    //Controls the Flight script on LeftHand and RightHand in Player.

    [Tooltip("How far the control stick or touchpad needs to be pressed to trigger flight.")]
    public float stickThreshold = 0.5f;

    [Tooltip("Set flight speed.")]
    public float flightSpeed = 50f;

    [Tooltip("How slow sideways flight should be compared to forward flight.")]
    public float sidewaysSlowdown = 0.25f;

    [Tooltip("Enable rotation.")]
    public bool rotationEnabled = true;

    [Tooltip("Number of degrees to rotate for each control stick input.")]
    public float rotateSpeed = 30f;

    [Tooltip("If true, you fly in the direction of your gaze. If false, you fly in the direction of your controller.")]
    public bool flyByGaze = true;

    [Tooltip("Show tooltips.")]
    public bool showTooltips = true;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
