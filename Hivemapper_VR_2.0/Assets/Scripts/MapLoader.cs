using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{
    //Loads maps from a list loaded into the Map Loader top level object.

    [Tooltip("The parent object for loaded maps.")]
    public GameObject MapParent;

    [Tooltip("The maps to be loaded.")]
    public GameObject[] Maps;

    [Tooltip("The height the user should start above the map.")]
    public float startingHeight = 5f;

    public GameObject Player;
    

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
        //Instantiate a map and adjust its size and position to be centered beneath the user at a standard size.
        //Expects Map Parent to be properly rotated and placed vertically.

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
        map.transform.position += Vector3.down * startingHeight; 
   
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
}
