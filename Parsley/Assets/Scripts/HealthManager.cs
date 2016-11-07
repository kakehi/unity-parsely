using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {

	public float health;

	// Receiving Damages
	public void GetDamage(float Damage){

		if (health > 0) {
			health -= Damage;
		} else {
			Die ();
		}
	}

	// Dying
	void Die(){
		Destroy (gameObject);
	}

}
