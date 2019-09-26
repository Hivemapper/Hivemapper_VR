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
        public SteamVR_Action_Boolean toggleRotationAction;

        public Hand hand;

        public GameObject flightController;

        private float stickThreshold;

        private bool showTooltips;

        public GameObject rotationIndicator;
        public GameObject strafeIndicator;

        private float flightSpeed;
        private float sidewaysSlowdown;
        private bool rotationEnabled;
        private float rotationSpeed;

        private Player player = null;
        private Vector3 direction;
        private Vector3 sideways;

        private bool justRotated = false;
        private Coroutine hintCoroutine = null;



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

            /*
            foreach (Hand hand in player.hands)
            {
                ControllerButtonHints.ShowTextHint(hand, flyAction, "Flight");
                Debug.Log("Active hint text: " + ControllerButtonHints.GetActiveHintText(hand, flyAction));
            }
              */  

            Invoke("ShowFlightHints", 0.0f);

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

            flyAction.AddOnUpdateListener(OnFlyAction, hand.handType);

            if (toggleRotationAction == null)
            {
                Debug.LogError(" < b >[SteamVR Interaction] </ b > No toggle rotation action assigned");
            }

            toggleRotationAction.AddOnChangeListener(OnToggleRotationAction, hand.handType);
        }


        private void OnDisable()
        {
            if (flyAction != null)
                flyAction.RemoveOnUpdateListener(OnFlyAction, hand.handType);

            if (toggleRotationAction != null)
            {
                toggleRotationAction.RemoveOnChangeListener(OnToggleRotationAction, hand.handType);
            }
        }

        private void Update()
        {
            //Set flight speed according to Flight Control.
            flightSpeed = flightController.GetComponent<FlightControl>().flightSpeed;
            sidewaysSlowdown = flightController.GetComponent<FlightControl>().sidewaysSlowdown;

            //Set rotation information according to Flight Control.
            rotationEnabled = flightController.GetComponent<FlightControl>().rotationEnabled;
            rotationSpeed = flightController.GetComponent<FlightControl>().rotateSpeed;

            //Set stick sensitivity according to Flight Control.
            stickThreshold = flightController.GetComponent<FlightControl>().stickThreshold;

            //Show relevant indicator.
            if (rotationEnabled && !rotationIndicator.activeSelf)
            {
                rotationIndicator.SetActive(true);
                strafeIndicator.SetActive(false);
            }

            else if (!rotationEnabled && rotationIndicator.activeSelf)
            {
                rotationIndicator.SetActive(false);
                strafeIndicator.SetActive(true);
            }

            //Get tooltip status 
            showTooltips = flightController.GetComponent<FlightControl>().showTooltips;
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



            //Toggles whether flight is controlled by gaze or controller.
            if (flightController.GetComponent<FlightControl>().flyByGaze)
            {
                direction = player.hmdTransform.forward;
                sideways = player.hmdTransform.right;
            }
            else
            {
                direction = this.transform.forward;
                sideways = this.transform.right;
            }

            //Changing from adjustable speed flight to constant speed flight
            //player.transform.position += direction * axis[1] * Time.deltaTime * flightSpeed;
            if (axis[1] >= stickThreshold)
                player.transform.position += direction * Time.deltaTime * flightSpeed;
            else if (axis[1] <= (0 - stickThreshold))
                player.transform.position -= direction * Time.deltaTime * flightSpeed;


            //If rotation is enabled, it should jump in increments.
            if (rotationEnabled)
            {
                //Only allow one rotation at a time.
                if (!justRotated)
                {
                    if (axis[0] >= stickThreshold)
                        player.transform.Rotate(0, rotationSpeed, 0);
                    else if (axis[0] <= (0 - stickThreshold))
                        player.transform.Rotate(0, 0 - rotationSpeed, 0);
                    justRotated = true;
                }

                //When the user releases the thumbstick back to neutral, allow rotation again.
                if (Math.Abs(axis[0]) < stickThreshold)
                    justRotated = false;
            }

            //If rotation is not enabled, translate user left or right on sideways joystick press.
            else
            {
                if (axis[0] >= stickThreshold)
                    player.transform.position += sideways * Time.deltaTime * flightSpeed * sidewaysSlowdown;
                else if (axis[0] <= (0 - stickThreshold))
                    player.transform.position -= sideways * Time.deltaTime * flightSpeed * sidewaysSlowdown;
                
            }

            //Debug.Log("Player position: " + player.transform.position);
            //Debug.Log("Axis[0]: " + axis[0]);
            //Debug.Log("Axis[1]: " + axis[1]);
        }


        private void OnToggleRotationAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, Boolean change)
        {
            if (change)
            {
                flightController.GetComponent<FlightControl>().rotationEnabled = !flightController.GetComponent<FlightControl>().rotationEnabled;
                Debug.Log("Rotation enabled: " + flightController.GetComponent<FlightControl>().rotationEnabled);
            }
        }


        public void ShowFlightHints()
        {
            CancelFlightHints();
            hintCoroutine = StartCoroutine(FlightHintsCoroutine());
        }

        public void CancelFlightHints()
        {
            if (hintCoroutine != null)
            {
                ControllerButtonHints.HideTextHint(player.leftHand, flyAction);
                ControllerButtonHints.HideTextHint(player.rightHand, flyAction);

                StopCoroutine(hintCoroutine);
                hintCoroutine = null;
            }

            CancelInvoke("ShowFlightHints");
        }

        private IEnumerator FlightHintsCoroutine()

        {
            //Show hints, without controller vibration
            while (true)
            {
                foreach (Hand hand in player.hands)
                {
                    bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, flyAction));
                    //Debug.Log("Active hint text: " + ControllerButtonHints.GetActiveHintText(hand, flyAction));

                    if (showTooltips && !isShowingHint)
                    {
                        ControllerButtonHints.ShowTextHint(hand, flyAction, "Fly/Rotate");
                        ControllerButtonHints.ShowTextHint(hand, toggleRotationAction, "Toggle\nRotation");
                    }


                    else if (!showTooltips && isShowingHint)
                    {
                        ControllerButtonHints.HideTextHint(hand, flyAction);
                        ControllerButtonHints.HideTextHint(hand, toggleRotationAction);
                        Debug.Log("Hints hidden.");
                    }
                }

                yield return null;
            }

            /*
            float prevBreakTime = Time.time;
            float prevHapticPulseTime = Time.time;
            
            while (true)
            {
                bool pulsed = false;


                //Show the hint on each eligible hand
                foreach (Hand hand in player.hands)
                {
                    bool showHint = true;//IsEligibleForTeleport(hand);
                    bool isShowingHint = !string.IsNullOrEmpty(ControllerButtonHints.GetActiveHintText(hand, flyAction));
                    if (showHint)
                    {
                        if (!isShowingHint)
                        {
                            ControllerButtonHints.ShowTextHint(hand, flyAction, "Flight");
                            prevBreakTime = Time.time;
                            prevHapticPulseTime = Time.time;
                        }

                        if (Time.time > prevHapticPulseTime + 0.05f)
                        {
                            //Haptic pulse for a few seconds
                            pulsed = true;

                            hand.TriggerHapticPulse(500);
                        }
                    }
                    else if (!showHint && isShowingHint)
                    {
                        ControllerButtonHints.HideTextHint(hand, flyAction);
                    }
                }

                if (Time.time > prevBreakTime + 3.0f)
                {
                    //Take a break for a few seconds
                    yield return new WaitForSeconds(3.0f);

                    prevBreakTime = Time.time;
                }

                if (pulsed)
                {
                    prevHapticPulseTime = Time.time;
                }

                yield return null;
            }
            */
        }

    }
}