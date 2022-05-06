using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObject : MonoBehaviour
{

	static readonly TerrainObject nil = new TerrainObject(0);
	private TerrainController controller;
	public int id;
	public int type;
	public GameObject renderer;

	// Start is called before the first frame update
	void Start(){
		controller = this.transform.parent.GetComponent<TerrainController>();
		id = controller.idNext;
		controller.idNext = id + 1;
		this.transform.position = new Vector3(Mathf.Floor(transform.position.x/controller.size)*controller.size-controller.size/2,transform.position.y,Mathf.Floor(transform.position.z/controller.size)*controller.size-controller.size/2);

		controller.terrainObjects.Add(transform.position,this);

	}

	// Update is called once per frame
	void Update(){
	}

	public void Refresh(){
		Destroy(renderer);
		renderer = Instantiate(controller.terrains[type],transform.position,Quaternion.identity);
		renderer.SetActive(true);
		renderer.transform.parent = this.transform;
	}

	//constructors
	public TerrainObject(int t){
		this.type = t;
	}

	//operators
	public static bool operator==(TerrainObject lhs, TerrainObject rhs){
		return (lhs.id == rhs.id);
	}

	public static bool operator!=(TerrainObject lhs, TerrainObject rhs){
		return !(lhs == rhs);
	}

	//functions
	TerrainObject Find(Vector2 position){
		TerrainObject result;
		if(controller.terrainObjects.TryGetValue(new Vector3(transform.position.x+position.x*controller.size,transform.position.y,transform.position.z+position.y*controller.size),out result)){
			return(result);
		}
		return TerrainObject.nil;
	}
}
