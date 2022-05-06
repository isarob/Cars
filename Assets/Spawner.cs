using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

	public int w;
	public int h;
	public GameObject obj;
	public int offset;
    // Start is called before the first frame update
    void Start()
    {
		for(int x=0; x<w; x++){
			for(int y=0; y<h; y++){
				GameObject newObj = Instantiate(obj, new Vector3(x*offset,0,y*offset),obj.transform.rotation);
				newObj.transform.parent = transform.parent;
				newObj.SetActive(true);
			}
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
