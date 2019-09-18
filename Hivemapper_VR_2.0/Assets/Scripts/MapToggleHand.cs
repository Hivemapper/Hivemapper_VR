using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Valve.VR.InteractionSystem
{
    public class MapToggleHand : MonoBehaviour
    {
        //Intended to be attached to hands; activates Map Loader script to load next map.

        public SteamVR_Action_Boolean mapToggleAction;
        public SteamVR_Action_Boolean changeDetectionToggleAction;

        public Hand hand;

        public GameObject mapLoader;

        private Player player = null;

        // Start is called before the first frame update
        void Start()
        {
            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> Map Toggle: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }
        }

        private void OnEnable()
        {
            if (hand == null)
                hand = this.GetComponent<Hand>();

            if (mapToggleAction == null || changeDetectionToggleAction == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No map toggle or change detection toggle action assigned");
                return;
            }

            mapToggleAction.AddOnChangeListener(OnMapToggleAction, hand.handType);
            changeDetectionToggleAction.AddOnChangeListener(OnChangeDetectionToggleAction, hand.handType);
        }


        private void OnDisable()
        {
            if (mapToggleAction != null)
                mapToggleAction.RemoveOnChangeListener(OnMapToggleAction, hand.handType);

            if (changeDetectionToggleAction != null)
                changeDetectionToggleAction.RemoveOnChangeListener(OnChangeDetectionToggleAction, hand.handType);
        }


        private void OnMapToggleAction(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
        {
            if (newValue)
            {
                MapLoader mapLoaderScript = mapLoader.GetComponent<MapLoader>();
                mapLoaderScript.ChangeMap();
            }
        }

        private void OnChangeDetectionToggleAction(SteamVR_Action_Boolean actionIn, SteamVR_Input_Sources inputSource, bool newValue)
        {
            if (newValue)
            {
                MapLoader mapLoaderScript = mapLoader.GetComponent<MapLoader>();
                mapLoaderScript.ToggleChangeDetection();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}