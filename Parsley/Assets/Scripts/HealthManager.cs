using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;


	// -- health counter
	public float health;
	float currentHealth;

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
	}

	// Receiving Damages
	public void GetDamage(float Damage){

		if (health > 0.2f) {

			currentHealth -= Damage;

			UpdateScale ();

		} else {
			Die ();
		}
	}


	// Unactivate the character and move them out of screen
	public void Die(){
		active = false;
		transform.position = new Vector3 (-2 * GMgameManager.rangeX, 0, 0);
	}


	void UpdateScale (){
		
		// Change Scale State
		if(transform.tag == "Alien")
			transform.localScale = new Vector3(currentHealth / 100 + 0.2f, currentHealth / 100 + 0.2f, currentHealth / 100 + 0.2f);

		if(transform.tag == "Player")
			transform.localScale = new Vector3(currentHealth / 1000 + 0.2f, currentHealth / 1000 + 0.2f, currentHealth / 1000 + 0.2f);
	}
}
