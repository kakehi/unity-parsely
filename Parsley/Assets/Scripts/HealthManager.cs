using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;

	// Sprite that appears when it dies
	public GameObject DeathEffect;
	float deathEffectValue = 0;

	// -- health counter
	public float health;
	public float currentHealth;

	// -- If the target can recover by itself
	public bool recoverable = false;

	// -- if the character is in screen or not
	public bool active = false;

	// -- Whether if it's on stage (Could be dead for after effects)
	public bool onStage = false;

	// -- show effects when get damage
	public bool showEffectPerDamage = false;
	bool gotDamage = false;

	// -- if just died, this bool becomes true. animate the effect and die.
	public bool justDied = false;

	// -- only if it is the first alien in the scene. It will die then it will begin the game.
	public bool introAlien = false;

	void Start(){
		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Updating Health
		currentHealth = health;

		// Randomize Death Effect Sprite
		if(DeathEffect != null)
			DeathEffect.transform.eulerAngles = new Vector3 (90, Random.Range(0,360), 0);

		UpdateScale ();

	}


	void Update(){

		if (recoverable && currentHealth < health && !justDied){
			currentHealth += 1.0f;
			UpdateScale ();
		}

		if(!recoverable && !justDied)
			UpdateScale ();

		// Show Effects
		if(showEffectPerDamage && gotDamage) {
			deathEffectValue += 0.03f;
			DeathEffect.transform.localScale = new Vector3 (deathEffectValue, deathEffectValue, deathEffectValue);
			DeathEffect.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1 - deathEffectValue/2f);
			if (deathEffectValue > 2.0f) {
				deathEffectValue = 0;
				gotDamage = false;
			}
		}

		// Death Effect
		if (DeathEffect != null && justDied) {
			deathEffectValue += 0.03f;
			DeathEffect.transform.localScale = new Vector3 (deathEffectValue, deathEffectValue, deathEffectValue);
			DeathEffect.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1 - deathEffectValue/2f);
			DeathEffect.transform.eulerAngles = new Vector3 (90, Random.Range(0,360), 0);
			// -- If it is plant... call the dead function and make them scale 0, randomly rotate;
			if (transform.tag == "PlantSeed") {
				DispatchAllChildPlants ();
				transform.localScale = new Vector3 (0, 0, 0);
			}

			if (deathEffectValue > 2.0f) {
				MakeDefault ();
				onStage = false;
			}
		}
	}

	// Receiving Damages
	public void GetDamage(float Damage){

		gotDamage = true;

		currentHealth -= Damage;

		if (currentHealth < .5f)
			Die ();
		else
			UpdateScale ();
	}


	// Unactivate the character and move them out of screen
	public void Die(){
		
		active = false;
		justDied = true;

		// If the gameobject has motion class, turn off
		if (transform.GetComponent<MovingMecanic> () != null)
			transform.GetComponent<MovingMecanic> ().inMotion = false;

		// If it's first alien, it will begin the game
		if (introAlien){
			GMgameManager.BeginGame ();
			introAlien = false;
		}
		
	}
		
	public void MakeDefault(){

		currentHealth = health;

		deathEffectValue = 0;
			
		justDied = false;

		transform.position = new Vector3 (-2 * GMgameManager.rangeX, 0, 0);

		if (DeathEffect != null) {
			DeathEffect.transform.localScale = new Vector3 (0, 0, 0);
		}

	}

	void UpdateScale (){
		
		// Change Scale State
		if(transform.tag == "Alien")
			transform.GetComponent<Alien>().alienSprite.transform.localScale = new Vector3(currentHealth / 50, currentHealth / 50, currentHealth / 50);

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

	void DispatchAllChildPlants (){
		int i = 0;
		while(i < GMgameManager.allPlants.Length){
			if(GMgameManager.allPlants[i].GetComponent<Plant>().myParentSeed == transform.gameObject)
				GMgameManager.allPlants[i].GetComponent<Plant> ().dispatchPlant ();
			i++;
		}
	}
}
