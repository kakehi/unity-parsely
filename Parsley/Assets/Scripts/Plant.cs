using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	bool spawned = false;
		
	public GameObject childPrefab;
	GameObject activeChildPrefab;

	// Growth Parameter
	public float branchDistance = 2.0f;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Random.Range (0.0f, 1.0f) > 0.95 && spawned == false)
			SpawnNext ();
	}

	void SpawnNext(){
		// -- Create New Position
		Vector3 newPos = Random.onUnitSphere * branchDistance;
		newPos.y = 0;
		//newPos += transform.position; // Add current Position

		// -- Spawn New Plant
		activeChildPrefab = Instantiate (childPrefab, transform.position + newPos, Quaternion.identity) as GameObject;
		activeChildPrefab.transform.parent = transform.parent;
		activeChildPrefab.transform.LookAt (transform.position);
		spawned = true;
	}
}
