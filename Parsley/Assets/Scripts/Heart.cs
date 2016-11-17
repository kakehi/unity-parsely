using UnityEngine;
using System.Collections;

public class Heart : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;

	public bool active = false;

	public float speed = 0.0005f;


	public float heartScale = 0.05f;
	public float currentHeartScale;
	public float heartGrowthScale = 0.5f;

	public GameObject graphicSprite;

	void Start(){

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Making Default Property
		MakeDefault ();
	}

	void Update(){

		// -- Update the scale of heart
		transform.localScale = new Vector3 (currentHeartScale, currentHeartScale, currentHeartScale);
		graphicSprite.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1.0f - currentHeartScale/3.0f);


		if(1.0f - currentHeartScale/3.0f < 0.05f){
			GMgameManager.SpawnSeeder (transform.position);
			MakeDefault();
		}
			

		// -- Moving when active

		if (active) {

			// Motion
			transform.position -= new Vector3 (0,0, speed);

			// Reach Out of Screen
			if ( - GMgameManager.rangeZ - 3.0f > transform.position.z) {
				MakeDefault();
			}
		}

	}


	void MakeDefault(){
		active = false;
		currentHeartScale = heartScale;
		transform.position = new Vector3(GMgameManager.rangeX * 2, 0, GMgameManager.rangeZ);
	}


	public void Onboarding (){

		active = true;

		//  Initially positioning
		transform.position = new Vector3( Random.Range (-GMgameManager.rangeX, GMgameManager.rangeX) * 0.75f, 0, GMgameManager.rangeZ + 3.0f);

	}

}
