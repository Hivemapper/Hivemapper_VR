using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    public class GripDetect : MonoBehaviour
    {

        public SteamVR_Action_Boolean gripAction;

        public Hand hand;

        private Player player = null;

        public bool isGripped = false;
        // Start is called before the first frame update
        void Start()
        {
            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> Grip Action: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            if (hand == null)
                hand = this.GetComponent<Hand>();

            if (gripAction == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No grip action assigned");
                return;
            }

            gripAction.AddOnUpdateListener(OnGripAction, hand.handType);
        }


        private void OnDisable()
        {
            if (gripAction != null)
                gripAction.RemoveOnChangeListener(OnGripAction, hand.handType);
        }


        private void OnGripAction(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool isPressed)
        {

            if (this.name == "LeftHand")
            {
                if (isPressed && inputSource.ToString() == "LeftHand")
                {
                    isGripped = true;
                }
                else
                    isGripped = false;
            }

            if (this.name== "RightHand")
            {
                if (isPressed && inputSource.ToString() == "RightHand")
                {
                    isGripped = true;
                }

                else
                    isGripped = false;
            }


        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("Source: " + this.name + "; IsGripped: " + isGripped);

        }
    }
}