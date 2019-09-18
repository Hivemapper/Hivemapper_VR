using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace Valve.VR.InteractionSystem
{
    public class Flight : MonoBehaviour
    {
        public SteamVR_Action_Vector2 flyAction;

        public Hand hand;

        public GameObject flightController;

        private float flightSpeed;
        private bool rotationEnabled;
        private float rotationSpeed;

        private Player player = null;

        // Start is called before the first frame update
        void Start()
        {
            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> Flight: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }


        }

        private void OnEnable()
        {
            if (hand == null)
                hand = this.GetComponent<Hand>();

            if (flyAction == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No fly action assigned");
                return;
            }

            flyAction.AddOnAxisListener(OnFlyAction, hand.handType);
        }


        private void OnDisable()
        {
            if (flyAction != null)
                flyAction.RemoveOnAxisListener(OnFlyAction, hand.handType);
        }

        private void OnFlyAction(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
        {
            /*
             * Debug logs
            Debug.Log("On Fly Action Change triggered.");
            Debug.Log("Action: " + fromAction);
            Debug.Log("Source: " + fromSource);
            Debug.Log("Axis: " + axis);
            Debug.Log("Delta: " + delta);
            */

            //Elevator script - moves only up and down
            //player.transform.position += Vector3.up * axis[1] * Time.deltaTime * flightSpeed;


            //Debug.Log("Source: " + fromSource);

            Vector3 direction = this.transform.forward;

            player.transform.position += direction * axis[1] * Time.deltaTime * flightSpeed;

            if (rotationEnabled)
            {
                player.transform.Rotate(0, Time.deltaTime * rotationSpeed * axis[0], 0);
            }

            //Debug.Log("Player position: " + player.transform.position);
            //Debug.Log("Axis[0]: " + axis[0]);
        }


        private void Update()
        {
            //Set flight speed according to Flight Control.
            flightSpeed = flightController.GetComponent<FlightControl>().flightSpeed;

            //Set rotation information according to Flight Control.
            rotationEnabled = flightController.GetComponent<FlightControl>().rotationEnabled;
            rotationSpeed = flightController.GetComponent<FlightControl>().rotateSpeed;



        }


    }
}