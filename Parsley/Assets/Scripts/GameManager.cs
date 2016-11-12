using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// -- Motion Range
	public float rangeX = 10.0f;
	public float rangeZ = 12.0f;

	// -- Plant Spawn
	public GameObject plantStorage;
	public GameObject[] plantSeeders;
	public GameObject[] allPlants;

	// -- Alien Spawn
	public GameObject[] allAliens;


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


			// -- For ones moving, move away
			i = 0;
			while (i < allPlants.Length) {
				Debug.Log (Vector3.Distance(allPlants [i].transform.position, Input.mousePosition));
				if (
					allPlants [i].GetComponent<HealthManager> ().active && 
					allPlants [i].GetComponent<MovingMecanic> ().inMotion &&
					Vector3.Distance(allPlants [i].transform.position, Input.mousePosition) < 200.0f
				) {
					// Look Away from mouseposition
					allPlants [i].transform.LookAt (2 * transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15)));
				}
				i++;
			}

		}
	}


	// Spawn Alien

	IEnumerator SpawnAlien(int spawnCounter){

		while (true) {
			
			int i = 0;
			while (i < spawnCounter) {

				// -- if alien is not currently active, activate them
				if (!allAliens [i].GetComponent<HealthManager> ().active) {
					allAliens [i].GetComponent<HealthManager> ().active = true;
					allAliens [i].GetComponent<MovingMecanic> ().Onboarding ();
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
