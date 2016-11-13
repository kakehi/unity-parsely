using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// -- Motion Range
	public float rangeX;
	public float rangeZ;

	// -- Plant Spawn
	public GameObject plantStorage;
	public GameObject[] plantSeeders;
	public GameObject[] allPlants;

	// -- Alien Spawn
	public GameObject[] allAliens;


	// -- Flocking
	public Vector3 goalPos = Vector3.zero;
	int goalPositionIntervalWithMouse = 0;

	// Use this for initialization
	void Start () {
		plantStorage = GameObject.FindGameObjectWithTag ("PlantStorage");
		plantSeeders = GameObject.FindGameObjectsWithTag ("PlantSeed");

		allPlants = GameObject.FindGameObjectsWithTag ("Plant");

		allAliens = GameObject.FindGameObjectsWithTag ("Alien");

		// -- Start Spawning Aliens
		StartCoroutine(SpawnAlien(5));

	}


	void Update(){
	
		// -- if tapped or clicked
		if (Input.GetMouseButtonDown (0)) {

			bool SeederWasPressed = false;

			int i = 0;
			while (i < plantSeeders.Length) {

				// -- checked if it is close to the seeder
				if (Vector3.Distance (plantSeeders [i].transform.position, Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15))) < 1.0f){

					SeederWasPressed = true;

					int j = 0;
					while (j < allPlants.Length) {
						// -- check if plants are active and if plant seeder is target
						if (allPlants [j].GetComponent<HealthManager> ().active && allPlants [j].transform.parent.transform == plantSeeders [i].transform.parent.transform)
							allPlants [j].GetComponent<Plant> ().dispatchPlant ();
						j++;
					}

					plantSeeders [i].GetComponent<Plant> ().dispatchPlant ();
				}
				i++;
			}


			// -- If it was pressed outside of seed, plants react to mouse.
			if (!SeederWasPressed) {

				// -- Update the Goal Pos to be Mouse
				goalPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15));
				// -- Update Interval
				goalPositionIntervalWithMouse = 500;
				// -- Update Alliens Accelerator
				for (int k = 0; k < plantSeeders.Length; k++) {
					if (allPlants [k].GetComponent<HealthManager> ().active){
						allPlants [k].GetComponent<MovingMecanic> ().flockingAccelerator = 2.5f;
						allPlants [k].GetComponent<MovingMecanic> ().flockingRotationAccelerator = 2.5f;
					}
				}

			}

			// -- For ones moving, move away
			/*i = 0;
			while (i < allPlants.Length) {
				if (
					allPlants [i].GetComponent<HealthManager> ().active && 
					allPlants [i].GetComponent<MovingMecanic> ().inMotion &&
					Vector3.Distance(allPlants [i].transform.position, Input.mousePosition) < 200.0f
				) {
					// Look Away from mouseposition
					allPlants [i].transform.LookAt (2 * transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15)));
				}
				i++;
			}*/

		}


		// -- Randomize Goal Position for Flocking
		if (Random.Range (0, 10000) < 50 && goalPositionIntervalWithMouse == 0) {
			goalPos = new Vector3 (Random.Range (-rangeX, rangeX), 0, Random.Range (-rangeZ, rangeZ));
		}

		// -- Updating Mouse Position Interval
		if (goalPositionIntervalWithMouse > 0)
			goalPositionIntervalWithMouse--;
	}


	// Spawn Alien

	IEnumerator SpawnAlien(int spawnCounter){

		while (true) {
			
			int i = 0;
			while (i < spawnCounter) {

				// -- if alien is not currently active, activate them
				if (!allAliens [i].GetComponent<HealthManager> ().active) {
					allAliens [i].GetComponent<HealthManager> ().active = true;
					allAliens [i].GetComponent<Entering> ().Onboarding ();
				}

				// -- check if alien's number reached all spare aliens
				if (i == allAliens.Length - 1) {
					i = spawnCounter;
				} else {
					i++;
				}
			}

			yield return new WaitForSeconds (8);

			if (spawnCounter < allAliens.Length)
				spawnCounter += Random.Range(-5,10);
		}

	}
		
		

}
