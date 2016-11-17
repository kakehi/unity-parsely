using UnityEngine;
using System.Collections;

public class Entering : MonoBehaviour {

	// -- Moving Mecanic
	public MovingMecanic MM;

	// -- If the player is entering from outside.
	public bool isEntering = false;

	// Entering Target Position From Outside
	Vector3 enteringTargetPos;


	// MOVE
	float movingSpeed;
	public float minSpeed = 2.0f;
	public float maxSpeed = 2.0f; // assign same value if to be consistent
	// -- Moving Duration
	float movingDuration;
	public float minMovingDuration = 2.0f;
	public float maxMovingDuration = 2.0f; // assign same value if to be consistent
	bool moving = false;
	// -- Moving Duration
	float movingCounter;
	public float minMovingCounter = 10.0f;
	public float maxMovingCounter = 10.0f; // assign same value if to be consistent
	// -- Rotation
	float targetRotation;
	float deltaRotation;


	// Use this for initialization
	void Start () {
		StartCoroutine (MovingUpdate());
	}
	
	void Update(){

		// ONBORDING
		// -- moves until target area
		if (isEntering) {

			transform.position += transform.forward * Time.deltaTime *movingSpeed;

			if (transform.position.x > -MM.rangeX * .8 && transform.position.x < MM.rangeX * .8 && transform.position.z > -MM.rangeZ * .8 && transform.position.z < MM.rangeZ * .8) {
				isEntering = false;
				MM.inMotion = true;
			}
		}

	}


	// MOVE
	void MoveUpdate(){

		if (minSpeed == maxSpeed && minMovingDuration == maxMovingDuration){
			movingSpeed = minSpeed;
			movingDuration = minMovingDuration;
			return;
		}

		// -- Assign moving speed
		if (minSpeed != maxSpeed)
			movingSpeed = Random.Range (minSpeed, maxSpeed);
		else
			movingSpeed = minSpeed;
		// -- Assign moving duration
		if (minMovingDuration != maxMovingDuration)
			movingDuration = Random.Range (minMovingDuration, maxMovingDuration);
		else
			movingDuration = minMovingDuration;
	}

	void RotationUpdate(){
		targetRotation = Random.Range(-2, 2);
		//targetRotation = new Vector3 (Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
	}

	IEnumerator MovingUpdate(){

		// -- Updates
		if(!moving)
			MoveUpdate();
		// -- Wait
		yield return new WaitForSeconds (2.0f);
		// -- UpdateCounter
		if (minMovingCounter != maxMovingCounter)
			movingCounter = Random.Range (minMovingCounter, maxMovingCounter);
		else
			movingCounter = minMovingCounter;
		moving = true;

	}


	// Entering Triggered by Manager
	public void Onboarding(){

		// Random Positioning
		int temp = Random.Range(0,2);
		if (temp < 1) {
			transform.position = new Vector3 (-1.2f * MM.rangeX, 0, Random.Range(-MM.rangeZ, MM.rangeZ) * 0.4f);
		} else if (temp < 2) {
			transform.position = new Vector3 (1.2f * MM.rangeX, 0, Random.Range(-MM.rangeZ, MM.rangeZ) * 0.4f);
		} else if (temp < 3) {
			transform.position = new Vector3 (Random.Range(-MM.rangeX, MM.rangeX), 0, -1.2f * MM.rangeZ);
		} else {
			transform.position = new Vector3 (Random.Range(-MM.rangeX, MM.rangeX), 0, 1.2f * MM.rangeZ);
		}

		// Entering
		enteringTargetPos = new Vector3(Random.Range(-MM.rangeX, MM.rangeX), 0, Random.Range(-MM.rangeZ, MM.rangeZ));
		transform.LookAt (enteringTargetPos);
		isEntering = true;
	}

}
