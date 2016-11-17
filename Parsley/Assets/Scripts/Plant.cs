using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;

	// -- boolean if this is the seed for the plant or branches
	public bool plantSeed = false;
	GameObject plantSeedGameObject;

	public GameObject myParentSeed;

	public bool spawneLimit = true;

	public float maxChild;

	public GameObject childPrefab;

	// -- Growth Parameter
	// -- branch length
	public float branchDistance;
	public float branchMinAnfgle;
	public float branchMaxAnfgle;



	public MovingMecanic ClassMM;



	// Use this for initialization
	void Start () {

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// -- Spawn Manage
		// -- if spawnseed turn limit off
		if(plantSeed){
			spawneLimit = false;
		}

		// -- Adjust the branch scale (inherit of the local scale)
		branchDistance *= transform.localScale.x;

		// -- Adjust the branch spawn angle
		if (plantSeed) {
			branchMinAnfgle = 0;
			branchMaxAnfgle = Mathf.PI*4;
		} else {
			branchMinAnfgle = Mathf.PI / 8 + transform.rotation.eulerAngles.y;
			branchMaxAnfgle = Mathf.PI * 7 / 8 + transform.rotation.eulerAngles.y;
		}


	}
	
	// Update is called once per frame
	void Update () {

		if (spawneLimit == false && maxChild > 0 && Random.Range(0,10) > 8 && transform.GetComponent<HealthManager>().active) {
			SpawnNext ();
		}


		// -- 
		if (ClassMM!= null && ClassMM.inMotion) {
			DeathManager ();
		}
			
	}

	void SpawnNext(){

		// -- Create New Position
		// -- Random on circle
		//Vector3 newPos = Random.onUnitSphere * branchDistance;
		// -- restrained area
		//Vector3 newPos = RandomCircle(branchDistance);
		// -- Random Location in Arc
		//Vector3 newPos = Quaternion.AngleAxis(Random.Range(-45.0f, 45.0f), transform.forward) * branchDistance;
		float angle = Random.Range(branchMinAnfgle, branchMaxAnfgle);
		Vector2 tempLoc = new Vector2(Mathf.Cos(angle) * branchDistance, Mathf.Sin(angle) * branchDistance);
		Vector3 newPos;
		newPos.x = tempLoc.x;
		newPos.y = 0;
		newPos.z = tempLoc.y;
		//newPos += transform.position; // Add current Position

		// -- Spawn New Plant
		int i=0;
		while(i < GMgameManager.allPlants.Length){
			if (!GMgameManager.allPlants [i].GetComponent<HealthManager> ().active) {
				Spawned (GMgameManager.allPlants [i], newPos);
				i = GMgameManager.allPlants.Length;
			}
			i++;
		}

	}

	void Spawned(GameObject activeChild, Vector3 newPos){

		activeChild.transform.position = newPos + transform.position;
		activeChild.transform.localScale = transform.localScale * 0.8f;
		activeChild.transform.LookAt (transform.position);

		activeChild.GetComponent<HealthManager> ().active = true;

		// -- Make child parent same as my parent
		if(plantSeed){
			activeChild.GetComponent<Plant> ().myParentSeed = transform.gameObject;
		}else{
			activeChild.GetComponent<Plant> ().myParentSeed = myParentSeed;
		}

		// -- ActiveChildLimit
		activeChild.GetComponent<Plant>().maxChild = (maxChild- Mathf.Abs(Random.Range(0,1)));

		// -- Randomize Sphere Size
		if(!activeChild.GetComponent<Plant>().plantSeed)
			activeChild.GetComponent<PlantDefenseUnit>().lifeSphere.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);


		// -- Adjust because spawned
		if (maxChild > 0) {
			maxChild--;
		} else {
			spawneLimit = true;
		}
	}

	// Dispatch plants

	public void dispatchPlant (){

		// -- make the max child count to default so it will spawn again
		if (plantSeed == true) {
			maxChild = 2;
			spawneLimit = false;
		} else {
			if(transform.GetComponent<HealthManager>().active == true){
				maxChild = 0;
				ClassMM.inMotion = true;
				spawneLimit = true;
			}

		}
	}


	Vector3 RandomCircle (float radius){
		float ang = transform.eulerAngles.y + Random.Range (-20.0f, 20.0f);
		Vector3 pos;
		pos.x = radius * Mathf.Sin(ang * Mathf.Deg2Rad);
		pos.y = 0;
		pos.z = radius * Mathf.Cos(ang * Mathf.Deg2Rad);
		return pos;
	}


	void DeathManager (){

		if (!plantSeed)
			transform.GetComponent<PlantDefenseUnit> ().DeathManager ();
	}

	void MakeDefault(){
		ClassMM.inMotion = false;
		spawneLimit = true;
		transform.localScale = new Vector3 (1, 1, 1);
		//maxChild = 0;
	}
}
