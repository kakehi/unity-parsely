using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;


	// -- health counter
	public float health;
	public float currentHealth;

	// -- If the target can recover by itself
	public bool recoverable = false;

	// -- if the character is in screen or not
	public bool active = false;


	void Start(){
		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Updating Health
		currentHealth = health;

		UpdateScale ();
	}


	void Update(){

		if (recoverable && currentHealth < health){
			currentHealth += 1.0f;
			UpdateScale ();
		}

		if(!recoverable)
			UpdateScale ();
	}

	// Receiving Damages
	public void GetDamage(float Damage){

		currentHealth -= Damage;

		if (currentHealth < .5f)
			Die ();
		else
			UpdateScale ();
	}


	// Unactivate the character and move them out of screen
	public void Die(){
		active = false;
		transform.position = new Vector3 (-2 * GMgameManager.rangeX, 0, 0);

		// If the gameobject has motion class, turn off
		if (transform.GetComponent<MovingMecanic> () != null)
			transform.GetComponent<MovingMecanic> ().inMotion = false;
	}


	void UpdateScale (){
		
		// Change Scale State
		if(transform.tag == "Alien")
			transform.localScale = new Vector3(currentHealth / 50, currentHealth / 50, currentHealth / 50);

		if(transform.tag == "Player")
			transform.localScale = new Vector3(currentHealth / 1000 + 0.2f, currentHealth / 1000 + 0.2f, currentHealth / 1000 + 0.2f);

		if (transform.tag == "PlantSeed") {
			transform.GetComponent<PlantSeedUnit> ().seeder.transform.localScale = new Vector3 (currentHealth/2.0f + 0.2f, currentHealth/2.0f + 0.2f, currentHealth/2.0f + 0.2f);
		}

		if (transform.tag == "Plant") {
			transform.GetComponent<PlantDefenseUnit> ().lifeSphereLocalScale = new Vector3 (currentHealth/2.0f, currentHealth/2.0f, currentHealth/2.0f);
			transform.GetComponent<PlantDefenseUnit> ().lifeBranchLocalScale = new Vector3 (currentHealth/2.0f, 2.0f, 1.0f);
		}

	}
}
