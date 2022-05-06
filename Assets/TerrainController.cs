using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{

	public GameObject[] terrains;
	public Dictionary <Vector3, TerrainObject> terrainObjects = new Dictionary<Vector3, TerrainObject>();

	public int size = 20;
	public int idNext = 1;

	public bool refresh = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(refresh){
			refresh = false;
			foreach(KeyValuePair<Vector3,TerrainObject> pair in terrainObjects){
				pair.Value.Refresh();
			}
		}
    }
}
