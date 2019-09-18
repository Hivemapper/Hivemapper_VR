using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLoader : MonoBehaviour
{
    //Loads maps from a list loaded into the Map Loader top level object.

    public GameObject Player;

    [Tooltip("The parent object for loaded maps.")]
    public GameObject MapParent;

    [Tooltip("The text display object.")]
    public GameObject TextDisplay;

    [Tooltip("The maps to be loaded.")]
    public GameObject[] Maps;

    [Tooltip("Change detection objects. Number should match with corresponding map.")]
    public GameObject[] ChangeDetectionObjects;

    [Tooltip("Change detection texture. Should be a transparent PNG matching map and change detection object.")]
    public Texture[] ChangeDetectionTextures;

    [Tooltip("The height the user should start above the map.")]
    public float startingHeight = 5f;

    [Tooltip("The material that change detection objects should use.")]
    public Material changeDetectionMaterial;

    [Tooltip("The transparent shader that Change Detection Maps should use.")]
    public Shader transparentShader;
    

    private int count = 0;

    void Start()
    {
        
        if (Maps.Length > 0)
        {
            LoadMap(Maps[count]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            ChangeMap();
        }
    }


    public void LoadMap(GameObject mapOriginal)
    {
        //Instantiate a map and adjust its size and position to be beneath the user at a standard size. The user should be standing on one edge.
        //Expects Map Parent to be properly rotated and scaled.

        GameObject map = Instantiate(mapOriginal, MapParent.transform, false);
        Mesh mapMesh = map.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        Bounds mapBounds = mapMesh.bounds;


        Debug.Log("Loaded map #" + count + ": " + map.name);
        
        //Map mesh information
        //Debug.Log("Map center: " + mapBounds.center);
        //Debug.Log("Map extents: " + mapBounds.extents);
        //Debug.Log("Map max: " + mapBounds.max);
        //Debug.Log("Map min: " + mapBounds.min);
        //Debug.Log("Map size: " + mapBounds.size);


        map.transform.localPosition = Player.transform.position - mapBounds.center;
        map.transform.localPosition -= new Vector3(mapBounds.extents.x, Vector3.zero.y, Vector3.zero.z);
        map.transform.position += Vector3.down * startingHeight;

        TextDisplay.transform.localPosition = Player.transform.position - new Vector3(4.0f, 0f, 0);
        TextDisplay.transform.LookAt(Player.transform);
        TextDisplay.transform.localPosition += new Vector3(0, 1f, 0);

        bool ChangeDetectionAvailable = false;
        //If there's an accompanying Change Detection, load it
        if (count > ChangeDetectionObjects.Length - 1 || count > ChangeDetectionTextures.Length - 1)
        {
            Debug.Log("Change Detection objects or maps list length does not match maps list length.");
        }
        else
        {
            if (ChangeDetectionObjects[count] != null && ChangeDetectionTextures[count] != null)
            {
                LoadChangeDetection(ChangeDetectionObjects[count], ChangeDetectionTextures[count]);
                ChangeDetectionAvailable = true;
            }

            else
                Debug.Log("No Change Detection object and texture for map " + map.name);
        }

        //Adjust text display
        TextDisplay.transform.Find("Canvas").Find("Map Name").gameObject.GetComponent<Text>().text = map.name;
        if (ChangeDetectionAvailable)
            TextDisplay.transform.Find("Canvas").Find("Availability").gameObject.GetComponent<Text>().text = "Yes";
        else
            TextDisplay.transform.Find("Canvas").Find("Availability").gameObject.GetComponent<Text>().text = "No";

    }

    public void ChangeMap()
    {
        foreach (Transform child in MapParent.transform)
        {
            Destroy(child.gameObject);
        }

        if (count >= Maps.Length - 1)
            count = 0;
        else
            count++;

        LoadMap(Maps[count]);
    }

    public void LoadChangeDetection(GameObject ChangeDetectionObjectOriginal, Texture ChangeDetectionTexture)
    {
        //Loads and then deactivates change detection object.

        GameObject changeDetectionMap = Instantiate(ChangeDetectionObjectOriginal, MapParent.transform, false);
        Mesh changeDetectionMesh = changeDetectionMap.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh;
        Bounds changeDetectionBounds = changeDetectionMesh.bounds;

        Debug.Log("Loaded change detection map for map " + MapParent.transform.GetChild(0).gameObject.name);

        changeDetectionMap.transform.localPosition = Player.transform.position - changeDetectionBounds.center;
        changeDetectionMap.transform.localPosition -= new Vector3(changeDetectionBounds.extents.x, Vector3.zero.y, Vector3.zero.z);
        changeDetectionMap.transform.position += Vector3.down * startingHeight;

        //Assign correct material and texture
        //changeDetectionMap.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = changeDetectionMaterial;
        changeDetectionMap.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.shader = transparentShader;
        changeDetectionMap.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.mainTexture = ChangeDetectionTexture;

        changeDetectionMap.SetActive(false);
    }

    public void ToggleChangeDetection()
    {
        //Toggles change detection map visibility.

        GameObject changeDetectionMap = null;

        if (MapParent.transform.childCount > 1)
            changeDetectionMap = MapParent.transform.GetChild(1).gameObject;

        if (changeDetectionMap != null)
        {
            if (changeDetectionMap.activeSelf)
                changeDetectionMap.SetActive(false);
            else
                changeDetectionMap.SetActive(true);

            Debug.Log("Change detection map visibility toggled.");
        }
        else
            Debug.Log("No change detection map found.");
    }

}
