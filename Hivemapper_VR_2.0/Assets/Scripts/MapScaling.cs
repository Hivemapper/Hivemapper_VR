using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MapScaling : MonoBehaviour
{
    public GameObject rightHand;
    public GameObject leftHand;
    public GameObject mapParent;

    private float controllerDistance;
    private float controllerDistanceReference;
    private Vector3 mapScaleReference;
    private Vector3 mapCenterReference;
    private bool bothGrippedOld = false;
    private bool bothGrippedNew = false;
    private bool manipulationMode = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calculate distance between controllers
        controllerDistance = Vector3.Distance(rightHand.transform.localPosition, leftHand.transform.localPosition);

        //Check whether both controllers are gripped.
        if (rightHand.GetComponent<GripDetect>().isGripped && leftHand.GetComponent<GripDetect>().isGripped)
        {
            //Debug.Log("Both controllers gripped.");
            bothGrippedNew = true;
        }
        else
            bothGrippedNew = false;

        //If both gripped status has changed to true, store controller distance and turn manipulation mode on
        //Also store current map information for reference
        if (bothGrippedNew && !bothGrippedOld)
        {
            controllerDistanceReference = controllerDistance;
            mapScaleReference = mapParent.transform.GetChild(0).transform.localScale;
            mapCenterReference = mapParent.transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshFilter>().mesh.bounds.center;
            manipulationMode = true;
            Debug.Log("Map manipulation mode engaged.");
        }

        //When the user releases either grip, turn off manipulation mode
        if (!bothGrippedNew && bothGrippedOld)
        {
            manipulationMode = false;
            Debug.Log("Map manipulation mode disengaged.");

        }

        //Finally, set bothGrippedOld to be equal to bothGrippedNew
        bothGrippedOld = bothGrippedNew;

        //Debug.Log("Manipulation mode: " + manipulationMode);
        //Debug.Log("Controller distance: " + controllerDistance);
        //Debug.Log("Controller distance reference: " + controllerDistanceReference);


        //If manipulation mode is on, change the scale of the map (and change detection map if present) according to the ratio of controller distance vs. stored controller distance
        if (manipulationMode)
        {
            foreach (Transform child in mapParent.transform)
            {
                child.localScale = mapScaleReference * controllerDistance / controllerDistanceReference;

                
            }

            Debug.Log("Map Scale: " + mapParent.transform.GetChild(0).transform.localScale);

        }


    }
}
