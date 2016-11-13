using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;

	// -- boolean if this is the seed for the plant or branches
	public bool plantSeed = false;
	GameObject plantSeedGameObject;

	public bool spawneLimit = true;

	public float maxChild;

	public GameObject childPrefab;
	GameObject activeChildPrefab;

	// -- Growth Parameter
	// -- branch length
	public float branchDistance;
	public float branchMinAnfgle;
	public float branchMaxAnfgle;

	// -- growing sphere
	float growingSpeed = 0.004f;
	float dyingSpeed = 0.003f;
	float deathAt = 0.002f;

	// -- growing branch
	float branchGrowingSpeed = 0.05f;

	public MovingMecanic ClassMM;


	// -- Plant in Motion
	public GameObject lifeBranch;
	public GameObject lifeSphere;

	public Vector3 lifeBranchLocalScale;
	public Vector3 lifeBranchAdditionalScaleAtAttack = new Vector3 (0,0,0);


	// Use this for initialization
	void Start () {

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// -- if it is not seed, grab seed.
		if (!plantSeed)
			plantSeedGameObject = GameObject.FindGameObjectWithTag ("PlantSeed");

		// -- Spawn Managers
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

	

		if (spawneLimit == false && maxChild > 0 && Random.Range(0,10) > 8) {
			SpawnNext ();
		}


		// -- 
		if (ClassMM!= null && ClassMM.inMotion) {
			DeathManager ();
		}

		// -- Grow If you are not plantSeed
		if (!plantSeed && transform.GetComponent<HealthManager> ().active) {
			if(lifeSphere.transform.localScale.x < 0.5f)
				lifeSphere.transform.localScale += new Vector3(growingSpeed, growingSpeed, growingSpeed);
			if(lifeBranchLocalScale.x < 0.5f)
				lifeBranchLocalScale += new Vector3(branchGrowingSpeed, 0, 0);
		}

		// -- When attach, change the form
		if (!plantSeed) {
			Debug.Log (lifeBranchLocalScale);
			lifeBranch.transform.localScale = lifeBranchLocalScale/* + lifeBranchAdditionalScaleAtAttack*/;
			if (lifeBranchAdditionalScaleAtAttack.y > 0)
				lifeBranchAdditionalScaleAtAttack -= new Vector3 (0, 0.02f, 0);
		}

		if(!plantSeed && transform.GetComponent<HealthManager>().active && lifeSphere.transform.localScale.x >0.2f && spawneLimit == true){
			spawneLimit = false;
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
				Spawned (i, newPos);
				i = GMgameManager.allPlants.Length;
			}
			i++;
		}

	}

	void Spawned(int i, Vector3 newPos){

		activeChildPrefab = GMgameManager.allPlants [i];

		activeChildPrefab.transform.position = newPos + transform.position;
		activeChildPrefab.transform.localScale = transform.localScale * 0.8f;
		activeChildPrefab.transform.LookAt (transform.position);

		activeChildPrefab.GetComponent<HealthManager> ().active = true;

		// -- Make child parent same as my parent
		activeChildPrefab.GetComponent<HealthManager> ().transform.parent = transform.parent;

		// -- ActiveChildLimit
		activeChildPrefab.GetComponent<Plant>().maxChild = (maxChild- Mathf.Abs(Random.Range(0,1)));
		// -- Randomize Sphere Size
		activeChildPrefab.GetComponent<Plant>().lifeSphere.transform.localScale = new Vector3(0, 0, 0);

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
			maxChild = 5;
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
		lifeSphere.transform.localScale -= new Vector3 (dyingSpeed, dyingSpeed, dyingSpeed);
		lifeBranchLocalScale -= new Vector3 (dyingSpeed/2, 0, 0);

		if (lifeSphere.transform.localScale.x * transform.localScale.x <= deathAt) {
			// convert the parent to storage
			transform.parent = GMgameManager.plantStorage.transform;

			MakeDefault ();
			transform.GetComponent<HealthManager> ().Die ();
		}

	}

	void MakeDefault(){
		ClassMM.inMotion = false;
		spawneLimit = true;
		transform.localScale = new Vector3 (1, 1, 1);
		lifeSphere.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		lifeBranchLocalScale = new Vector3 (0.05f, 2.0f, 1.0f);
		maxChild = 0;
	}
}
