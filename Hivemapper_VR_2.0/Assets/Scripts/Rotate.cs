using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{

    public class Rotate : MonoBehaviour
    {
        //Rotates player 1/4 turn clockwise while fading out the screen. 

        public SteamVR_Action_Boolean rotateAction;

        public Hand hand;

        private Player player = null;



        void Start()
        {
            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> Rotate: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            if (hand == null)
                hand = this.GetComponent<Hand>();

            if (rotateAction == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No map toggle action assigned");
                return;
            }

            rotateAction.AddOnChangeListener(OnRotateAction, hand.handType);
        }


        private void OnDisable()
        {
            if (rotateAction != null)
                rotateAction.RemoveOnChangeListener(OnRotateAction, hand.handType);
        }


        private void OnRotateAction(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
        {
             if(newValue)
            {
                player.transform.Rotate(0, 90, 0);
                Debug.Log("Rotation triggered.");
            }
        }
    }
}