using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace Valve.VR.InteractionSystem
{


public class MapLoader_Tiled : MonoBehaviour
{
    //Loads tiled maps from a specified folder.

    public GameObject Player;

    [Tooltip("The folder full of tiled maps to be loaded. Maps must be in Resources. " +
        "\n\nIf your maps are in Assets>Resources>Sample Data>Maps>tiled_meshes/0, " +
        "this string should be 'Sample Data/Maps/tiled_meshes/0'.")]
    public string FolderPath;

    [Tooltip("The parent object for loaded maps.")]
    public GameObject MapParent;

    [Tooltip("The text display object.")]
    public GameObject TextDisplay;

    [Tooltip("The height the user should start above the map.")]
    public float startingHeight = 5f;

    private Object[] Maps;

        List<Bounds> BoundsList = new List<Bounds>();

    // Start is called before the first frame update
    void Start()
    {
        LoadTiledMap(FolderPath);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadTiledMap(string FolderPath)
    {
            //Load all maps from a given folder. 
        Maps = Resources.LoadAll(FolderPath, typeof(GameObject));
        if (Maps.Length == 0)
            Debug.LogError("<b>No maps found at that folder path!</b>");
        
        //Instantiate a copy of each map tile.
        foreach (Object map in Maps)
        {
            Instantiate(map, MapParent.transform, false);
        }

        //Add necessary colliders and calculate the bounds of the full map.
        foreach (Transform map in MapParent.transform)
        {
                map.GetChild(0).gameObject.AddComponent<MeshCollider>();
                map.GetChild(0).gameObject.AddComponent<TeleportAreaInvisible>();
                map.GetChild(0).gameObject.GetComponent<TeleportAreaInvisible>().markerActive = false;

                //Mesh mapMesh = map.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
                //Bounds mapBounds = mapMesh.bounds;
                Bounds mapBounds = map.transform.GetChild(0).gameObject.GetComponent<Collider>().bounds;

                /*
                Debug.Log("Map:" + map.name);
                Debug.Log("Map center: " + mapBounds.center);
                Debug.Log("Map extents: " + mapBounds.extents);
                Debug.Log("Map max: " + mapBounds.max);
                Debug.Log("Map min: " + mapBounds.min);
                Debug.Log("Map size: " + mapBounds.size);
                */
                BoundsList.Add(mapBounds);

        }

            Vector3 boundsMax = FindBoundsMax(BoundsList);
            Vector3 boundsMin = FindBoundsMin(BoundsList);
            Vector3 boundsCenter = FindCenter(boundsMax, boundsMin);

            Debug.Log("Max: " + boundsMax);
            Debug.Log("Min: " + boundsMin);
            Debug.Log("Center: " + boundsCenter);

            //Move the map to be where the player is looking and below the player.
            
            //Center
            MapParent.transform.localPosition = Player.transform.position - boundsCenter;

            //Get camera direction
            Vector3 lookDirection = Player.GetComponent<Player>().hmdTransform.forward;
            lookDirection.y = 0; //Don't want to move it up or down
            lookDirection = Vector3.Normalize(lookDirection); //Want to keep the magnitude 1, but without y component
            Debug.Log("Look direction: " + lookDirection);

            //Move map in direction of camera direction and down
            MapParent.transform.position += lookDirection * startingHeight * 2;
            MapParent.transform.position += Vector3.down * startingHeight;

            //Move text display as well
            TextDisplay.transform.localPosition = Player.transform.position + lookDirection * 4f;
            TextDisplay.transform.LookAt(Player.transform);
            TextDisplay.transform.localPosition += new Vector3(0, 1f, 0);


            //Adjust text display
            TextDisplay.transform.Find("Canvas").Find("Map Name").gameObject.GetComponent<Text>().text = FolderPath;
            TextDisplay.transform.Find("Canvas").Find("Count").gameObject.GetComponent<Text>().text = Maps.Length.ToString();


        }

        public Vector3 FindBoundsMax (List<Bounds> BoundsList)
        {
            //Returns the highest x, y, and z bounds from a given list as a vector3

            float xMax = BoundsList[0].max.x;
            float yMax = BoundsList[0].max.y;
            float zMax = BoundsList[0].max.z;

            foreach (Bounds bounds in BoundsList)
            {
                if (bounds.max.x > xMax)
                    xMax = bounds.max.x;
                if (bounds.max.y > yMax)
                    yMax = bounds.max.y;
                if (bounds.max.z > zMax)
                    zMax = bounds.max.z;
            }

            return new Vector3(xMax, yMax, zMax);
        }

        public Vector3 FindBoundsMin(List<Bounds> BoundsList)
        {
            //Returns the lowest x, y, and z bounds from a given list as a vector3

            float xMin = BoundsList[0].min.x;
            float yMin = BoundsList[0].min.y;
            float zMin = BoundsList[0].min.z;

            foreach (Bounds bounds in BoundsList)
            {
                if (bounds.min.x < xMin)
                    xMin = bounds.min.x;
                if (bounds.max.y > yMin)
                    yMin = bounds.min.y;
                if (bounds.max.z > zMin)
                    zMin = bounds.min.z;
            }

            return new Vector3(xMin, yMin, zMin);
        }

        public Vector3 FindCenter(Vector3 max, Vector3 min)
        {
            //Given a maximum and minimum extents, finds the center
            float centerX = (max.x + min.x) / 2;
            float centerY = (max.y + min.y) / 2;
            float centerZ = (max.z + min.z) / 2;
            return new Vector3(centerX, centerY, centerZ);
        }

    }
}