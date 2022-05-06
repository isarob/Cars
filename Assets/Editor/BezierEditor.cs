using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bezier))]
[CanEditMultipleObjects]
public class BezierEditor : Editor 
{
	SerializedProperty lookAtPoint;

	void OnEnable()
	{
		
	}

	public override void OnInspectorGUI()
	{
		Bezier b = (Bezier) target;
		base.DrawDefaultInspector();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Add Point")) 
		{
			GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			DestroyImmediate(sphere.GetComponent<Collider>());
			sphere.layer = 9;
			sphere.name = (b.positions.Count/2).ToString()+" Node";
			sphere.transform.localScale = new Vector3(0.3f,0.3f,0.3f);
			sphere.transform.position = b.transform.position + Vector3.forward*Mathf.RoundToInt(b.positions.Count/2)+ Vector3.forward+Vector3.right;
			Material yellow = Resources.Load("EDITOR_Yellow", typeof(Material)) as Material;
			sphere.GetComponent<Renderer>().material = yellow;
			sphere.transform.parent = b.transform;
			b.positions.Add(sphere);


			GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			DestroyImmediate(cube.GetComponent<Collider>());
			cube.layer = 9;
			cube.name = (b.positions.Count/2).ToString()+" Cube";
			cube.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
			cube.transform.position = b.transform.position + Vector3.forward*Mathf.RoundToInt(b.positions.Count/2) + Vector3.forward;
			Material newMat = Resources.Load("EDITOR_Red", typeof(Material)) as Material;
			cube.GetComponent<Renderer>().material = newMat;
			cube.transform.parent = b.transform;
			b.positions.Add(cube);
			Debug.Log("Point Added");
		}

		if (GUILayout.Button("Remove Point")) 
		{
			DestroyImmediate(b.positions[b.positions.Count-1]);
			b.positions.RemoveAt(b.positions.Count-1);
			DestroyImmediate(b.positions[b.positions.Count-1]);
			b.positions.RemoveAt(b.positions.Count-1);
		}

		GUILayout.EndHorizontal();
	}
}