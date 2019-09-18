using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Valve.VR.InteractionSystem.Sample
{
    public class ControllerHints : MonoBehaviour
    {
        public Hand hand;


        // Start is called before the first frame update
        void Start()
        {
            //ControllerButtonHints.ShowTextHint
            for (int actionIndex = 0; actionIndex < SteamVR_Input.actionsIn.Length; actionIndex++)
            {
                //Debug.Log("Action index: " + SteamVR_Input.actionsIn[actionIndex]);

                ISteamVR_Action_In action = SteamVR_Input.actionsIn[actionIndex];

                //Debug.Log("Action: " + action);

                //if (action.GetActive(hand.handType))
                if (hand.handType == SteamVR_Input_Sources.RightHand)
                {
                    ControllerButtonHints.ShowTextHint(hand, action, action.GetShortName());
                    Debug.Log("Show text hint: " + action.GetShortName());
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}