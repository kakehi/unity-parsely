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

		Debug.Log ("Scale: " + transform.localScale + ", Alpha: " + graphicSprite.GetComponent<SpriteRenderer> ().material.color.a);
		// -- Update the scale of heart
		transform.localScale = new Vector3 (currentHeartScale, currentHeartScale, currentHeartScale);
		graphicSprite.GetComponent<SpriteRenderer> ().material.color = new Color (1f, 1f, 1f, 1.0f - currentHeartScale/2);


		if(1.0f - currentHeartScale/2 < 0.05f){
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
		GMgameManager.SpawnSeeder (transform.position);
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
