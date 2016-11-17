using UnityEngine;
using System.Collections;

public class PlantDefenseUnit : MonoBehaviour {


	// Game Manager
	GameManager GMgameManager;

	// -- growing branch
	float branchGrowingSpeed = 0.02f;


	// -- growing sphere
	float growingSpeed = 0.01f;
	float dyingSpeed = 0.005f;
	float deathAt = 0.01f;


	// -- Plant in Motion
	public GameObject lifeBranch;
	public GameObject lifeSphere;

	public Vector3 lifeSphereLocalScale;

	public Vector3 lifeBranchLocalScale;
	public Vector3 lifeBranchAdditionalScaleAtAttack = new Vector3 (0,0,0);

	// Use this for initialization
	void Start () {

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Initialize
		MakeDefault();
	}
	
	// Update is called once per frame
	void Update () {


		// -- Grow If you are not plantSeed
		if (transform.GetComponent<HealthManager> ().active) {
			lifeSphere.transform.localScale = lifeSphereLocalScale;
			lifeBranch.transform.localScale = lifeBranchLocalScale;

			// -- If it is NOT in motion yet (part of plant), keep growing, otherwise reaching to dye
			if (!transform.GetComponent<MovingMecanic> ().inMotion) {

				if (transform.GetComponent<HealthManager> ().currentHealth < 1.5f) {
					transform.GetComponent<HealthManager> ().currentHealth += growingSpeed;
				}
			} else {
				transform.GetComponent<HealthManager> ().currentHealth -= dyingSpeed;
			}

			/*if(lifeSphere.transform.localScale.x < 3.5f)
				transform.GetComponent<HealthManager> ().health += growingSpeed;
			if(lifeBranchLocalScale.x < 0.5f)
				lifeBranchLocalScale += new Vector3(branchGrowingSpeed, 0, 0);*/
		}

		// -- When detect, change the form
		lifeBranch.transform.localScale = lifeBranchLocalScale + lifeBranchAdditionalScaleAtAttack;
		if (lifeBranchAdditionalScaleAtAttack.x > 0){
			lifeBranchAdditionalScaleAtAttack -= new Vector3 (0.02f, 0, 0);
		}

		if(transform.GetComponent<HealthManager>().active && lifeSphere.transform.localScale.x >0.2f && transform.GetComponent<Plant>().spawneLimit == true){
			transform.GetComponent<Plant>().spawneLimit = false;
		}


	}
	
	

	public void DeathManager (){
		lifeSphere.transform.localScale -= new Vector3 (dyingSpeed, dyingSpeed, dyingSpeed);
		lifeBranchLocalScale -= new Vector3 (dyingSpeed/2, 0, 0);

		if (lifeSphere.transform.localScale.x * transform.localScale.x <= deathAt) {
			// convert the parent to storage
			transform.parent = GMgameManager.plantStorage.transform;

			MakeDefault ();
			transform.GetComponent<HealthManager> ().Die ();
			transform.GetComponent<HealthManager> ().MakeDefault ();
		}

	}



	public void MakeDefault(){
		lifeSphereLocalScale = new Vector3 (0.1f, 0.1f, 0.1f);
		lifeBranchLocalScale = new Vector3 (0.25f, 2.0f, 1.0f);
	}


}
