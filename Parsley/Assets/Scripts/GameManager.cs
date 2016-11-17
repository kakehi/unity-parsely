using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	// -- Motion Range
	public float rangeX;
	public float rangeZ;

	// -- Plant Spawn
	public GameObject activePlant;
	public GameObject plantStorage;
	public GameObject plantSeedStorage;
	public GameObject[] plantSeeders;
	public GameObject[] allPlants;

	// -- Alien Spawn
	public GameObject[] allAliens;

	// -- Heart
	public GameObject[] allHearts;
	public int[] HeartsAppearanceTimings;
	int currentHeartSpawnedCount = 0;

	// -- Opening Letter
	public GameObject[] OpeningLetters;

	// -- Flocking
	public Vector3 goalPos = Vector3.zero;
	public int goalPositionIntervalWithMouse = 0;

	// -- Mouse
	public Vector3 currentMousePosition;

	// -- UI Element
	public GameObject UISliderSprite;
	float UISliderScaleRate;
	float UISliderPosRate;
	float UIHeartRate;
	public GameObject UIHeartTimingPrefab;
	// -- Time when game ends
	public float TotalGameTime;
	float GameTimeCounter;

	// -- Opening
	bool opening = true;
	int letterCounter = 7;

	// Use this for initialization
	void Start () {

		// CREATING GAME OBJECTS BY TAGS

		// -- Opening Letters
		OpeningLetters = GameObject.FindGameObjectsWithTag ("Letter");

		// -- Where active plants stay
		activePlant = GameObject.FindGameObjectWithTag ("ActivePlant");
		// -- Srorage
		plantStorage = GameObject.FindGameObjectWithTag ("PlantStorage");
		plantSeedStorage = GameObject.FindGameObjectWithTag ("PlantSeedStorage");
		// -- All Plant Seeders List
		plantSeeders = GameObject.FindGameObjectsWithTag ("PlantSeed");
		// -- All Plants List
		allPlants = GameObject.FindGameObjectsWithTag ("Plant");

		allAliens = GameObject.FindGameObjectsWithTag ("Alien");

		allHearts = GameObject.FindGameObjectsWithTag ("Heart");

		Opening ();

		//BeginGame ();



		// UI ELEMENT

		// Rate of Game Time
		UISliderScaleRate = 1.0f/TotalGameTime;
		UISliderPosRate = 6.22f/TotalGameTime;

		// Rate of Heart Spawner
		int HeartTimes = 0;
		for (int i = 0; i < HeartsAppearanceTimings.Length; i++) {
			HeartTimes += HeartsAppearanceTimings [i];
		}
		UIHeartRate = HeartTimes/TotalGameTime;

		// -- Make UI Default
		MakeUIDefault();





	}


	void Update(){


		bool SeederWasPressed = false;

		// -- if tapped or clicked
		if (Input.GetMouseButtonDown (0)) {
			
			// -- Update my mouse position
			currentMousePosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15));

			// -- If it's at the opening, check if any plants are close to the Input
			if (letterCounter > 0) {

				int i = 0;

				while (i < allPlants.Length) {

					// -- checked if it is close to the seeder
					if (Vector3.Distance (allPlants[i].transform.position, currentMousePosition) < 1.2f) {

						if (!allPlants[i].GetComponent<MovingMecanic>().inMotion && allPlants[i].transform.parent.tag != null && allPlants[i].transform.parent.tag == "Letter") {
							SpawnLetter (allPlants[i].transform.parent.transform.gameObject);
							SeederWasPressed = true;
							i = allPlants.Length;
						}
							
					}

					i++;
				}

			} else {

				// -- If it's after game begins, check if plant seeder is close to the input
				int i = 0;
				while (i < plantSeeders.Length) {

					// -- checked if it is close to the seeder
					if (Vector3.Distance (plantSeeders [i].transform.position, currentMousePosition) < 1.0f) {

						SeederWasPressed = true;

						int j = 0;
						while (j < allPlants.Length) {

							if (opening) {
								// -- Only if during Openin

							} else {

								// -- Only if game begins

								// -- check if plants are active and if plant seeder is target
								if (allPlants [j].GetComponent<HealthManager> ().active && allPlants [j].GetComponent<Plant> ().myParentSeed == plantSeeders [i])
									allPlants [j].GetComponent<Plant> ().dispatchPlant ();
							}

							j++;
						}

						plantSeeders [i].GetComponent<Plant> ().dispatchPlant ();
					}
					i++;
				}
			}

			// -- If it was pressed outside of seed, plants react to mouse.
			if (!SeederWasPressed) {

				// -- Update the Goal Pos to be Mouse
				goalPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 15));
				// -- Update Interval
				goalPositionIntervalWithMouse = 1;
				// -- Update Alliens Accelerator
				for (int k = 0; k < plantSeeders.Length; k++) {
					if (allPlants [k].GetComponent<HealthManager> ().active){
						allPlants [k].GetComponent<MovingMecanic> ().flockingAccelerator = 1.2f;
						allPlants [k].GetComponent<MovingMecanic> ().flockingRotationAccelerator = 2.5f;
					}
				}

			}


		}



		// Make Default Location
		if (Input.GetMouseButton (0) && !SeederWasPressed) {
			goalPositionIntervalWithMouse = 1;
		} else {
			goalPositionIntervalWithMouse = 0;
		}
		// -- If it's opening, do not affect
		if(opening)
			goalPositionIntervalWithMouse = 0;


		// -- Randomize Goal Position for Flocking
		if (Random.Range (0, 10000) < 50 && goalPositionIntervalWithMouse == 0) {
			goalPos = new Vector3 (Random.Range (-rangeX, rangeX), 0, Random.Range (-rangeZ, rangeZ));
		}
	

		// -- Update UI Element
		UpdateUI();
	}


	// JUST FOR OPENING

	void Opening(){

		int i = 0;

		while(i<allPlants.Length){
			float tempScale = Random.Range (1, 5);
			allPlants [i].transform.eulerAngles = new Vector3(0, Random.Range (0, 360), 0); 
			//allPlants [i].GetComponent<PlantDefenseUnit> ().lifeSphere.transform.localScale = new Vector3 (0, 0, 0);
			allPlants [i].GetComponent<HealthManager> ().active = true;
			allPlants [i].GetComponent<HealthManager> ().health = 0;
			i++;
		}


	}

	void SpawnLetter(GameObject LetterParent){
		
		for (int i = 0; i < allPlants.Length; i++) {
			if (allPlants [i].transform.parent.transform.gameObject == LetterParent)
				allPlants [i].GetComponent<Plant> ().dispatchPlant ();
		}

		letterCounter--;

		if (letterCounter == 0) {
			
			// -- Start Spawning Aliens
			SpawnAlien(1);

			opening = false;

		}

	}

	public void BeginGame (){

		// -- Start Spawn Plant Seeder
		for(int i=0; i<2; i++){
			SpawnSeeder(new Vector3 (Random.Range (-rangeX, rangeX) * 0.5f, 0, Random.Range (-rangeZ, rangeZ) * 0.3f));
		}


		// -- Start Spawning Aliens
		StartCoroutine(SpawnAlienLogic(5));


		// -- Start Spawning Heart
		StartCoroutine(SpawnHeart());

	}

	// SPAWN SEEDER

	public void SpawnSeeder(Vector3 pos){

		int i = 0;
		while (i<plantSeeders.Length) {
			// -- if alien is not currently active, activate them
			if (!plantSeeders [i].GetComponent<HealthManager> ().active) {
				plantSeeders [i].GetComponent<HealthManager> ().active = true;
				plantSeeders [i].GetComponent<Plant> ().maxChild = 5;
				plantSeeders [i].transform.position = pos;
				plantSeeders [i].transform.parent = activePlant.transform;
				i = plantSeeders.Length;
			}

			i++;
		}

	}



	// SPAWN ALIEN

	IEnumerator SpawnAlienLogic(int spawnCounter){

		while (true) {

			SpawnAlien (spawnCounter);

			yield return new WaitForSeconds (32);
		}

	}

	void SpawnAlien(int spawnCounter){

		int i = 0;
		while (i < spawnCounter) {

			// -- if alien is not currently active, activate them
			if (!allAliens [i].GetComponent<HealthManager> ().onStage) {
				allAliens [i].GetComponent<HealthManager> ().onStage = true;
				allAliens [i].GetComponent<HealthManager> ().active = true;

				// -- If it's the first alien, flag so it will begin the game when it dies.
				if(letterCounter == 0 && opening){
					allAliens [i].GetComponent<HealthManager> ().introAlien = true;
				}

				allAliens [i].GetComponent<Entering> ().Onboarding ();
			}

			// -- check if alien's number reached all spare aliens
			if (i == allAliens.Length - 1) {
				i = spawnCounter;
			} else {
				i++;
			}
		}

	}


		
	// SPAWN ALIEN

	IEnumerator SpawnHeart(){

		while (true) {

			yield return new WaitForSeconds (HeartsAppearanceTimings[currentHeartSpawnedCount]);

			int i = 0;
			while (i < allHearts.Length) {

				// -- if heart is not currently active, activate them
				if (!allHearts [i].GetComponent<Heart> ().active) {
					allHearts [i].GetComponent<Heart> ().Onboarding ();
					i = allHearts.Length;
				} else {
					i++;
				}
			}



			// Move to next waiting time.
			currentHeartSpawnedCount++;
		}
	}



	// UI ELEMENT, Make Default

	void MakeUIDefault(){
		UISliderSprite.transform.localScale = new Vector3 (0f, 0.1f, 0.2f);
		UISliderSprite.transform.position = new Vector3 (0, 1f, 1.11f);
		GameTimeCounter = 0;

		// Make Hear Timing UI
		for (int i = 0; i < HeartsAppearanceTimings.Length; i++) {
			GameObject TempUI = Instantiate (UIHeartTimingPrefab);
			TempUI.transform.parent = UISliderSprite.transform.parent;
			TempUI.transform.localPosition = new Vector3 (HeartsAppearanceTimings[i] * UIHeartRate * 6.21f, 1.0f, 0);
		}

	}


	// UI ELEMENT, Update

	void UpdateUI(){

		if (GameTimeCounter < TotalGameTime)
			GameTimeCounter++;

		// -- Update Slider Scale and Position as Slider Progresses
		UISliderSprite.transform.localScale = new Vector3 (GameTimeCounter * UISliderScaleRate, 0.1f, 0.2f);
		UISliderSprite.transform.localPosition = new Vector3 (GameTimeCounter * UISliderPosRate, 1f, 1.11f);
	}

}
