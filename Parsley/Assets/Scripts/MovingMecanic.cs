using UnityEngine;
using System.Collections;

public class MovingMecanic : MonoBehaviour {

	public CharacterController cc;

	// Game Manager
	GameManager GMgameManager;

	// -- Target Object is Found or Not
	public bool targetFound = false;

	// -- In Motion (Plant doesn't move until motion)
	public bool inMotion = false;

	// -- If the player is entering from outside.
	public bool isEntering = false;


	// Entering
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

	// -- Motion Range
	float rangeX ,rangeZ;



	// TARGET
	public Vector3 targetPosition;

	// -- Target Tag
	public string targetTag;

	// -- Target Check
	float rayTraceSize = 10.0f;

	// -- boolean to check if the target is close enough
	bool isDamageable = false;
	float damageableRange = 1.5f;

	// -- Layers
	public LayerMask targetLayer;


	// Use this for initialization
	void Start () {

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Area Range
		rangeX = GMgameManager.rangeX;
		rangeZ = GMgameManager.rangeZ;


		StartCoroutine (MovingUpdate());
	}
	
	// Update is called once per frame
	void Update () {


		// -- if 2D
		//transform.eulerAngles.z = transform.eulerAngles.x = 0;
		//transform.position.y = 0;


		// ONBORDING
		// -- moves until target area
		if (isEntering) {
			cc.Move (transform.forward * movingSpeed * Time.deltaTime);

			if (transform.position.x > -rangeX * .8 && transform.position.x < rangeX * .8 && transform.position.z > -rangeZ * .8 && transform.position.z < rangeZ * .8) {
				isEntering = false;
				inMotion = true;
			}
		}

		// MOVE
		// Move only if it is in motion
		if (inMotion) {
			
			// Random MOVE
			if (!targetFound) {
				// -- Move
				if (moving) {
					if (movingSpeed > 0) {
						movingSpeed -= 0.05f;
					}
				}

				// -- Counting
				if (movingCounter > 0)
					movingCounter--;
				else if (moving) {
					moving = false;
					StartCoroutine (MovingUpdate ());
				}

				// Rotate
				if (Mathf.Abs (targetRotation - transform.rotation.y) > 0.1f)
					deltaRotation = targetRotation - transform.rotation.y;

				transform.RotateAround (transform.position, Vector3.up, deltaRotation * 100.0f * Time.deltaTime);

			} else {

				// Move Toward
				transform.LookAt (targetPosition);
				cc.Move (transform.forward * movingSpeed * 0.9f * Time.deltaTime);
			}


			// MOVE
			if (rangeX > transform.position.x && -1 * rangeX < transform.position.x && rangeZ > transform.position.z && -1 * rangeZ < transform.position.z) {
				//Debug.Log ("In the range");
				cc.Move (transform.forward * movingSpeed * Time.deltaTime);
			} else if (
				// Relieving from left
				rangeX <= transform.position.x && transform.eulerAngles.y > 225 && transform.eulerAngles.y < 315 ||
				// Relieving from right
				-1 * rangeX >= transform.position.x && transform.eulerAngles.y > 45 && transform.eulerAngles.y < 135 ||
				// Relieving from top
				rangeZ <= transform.position.z && transform.eulerAngles.y > 135 && transform.eulerAngles.y < 225 ||
				// Relieving from bottom Rotation 1
				-1 * rangeZ >= transform.position.z && transform.eulerAngles.y < 45 ||
				// Relieving from bottom Rotation 2
				-1 * rangeZ >= transform.position.z && transform.eulerAngles.y > 315) {
				//Debug.Log ("Recovering");
				cc.Move (transform.forward * movingSpeed * Time.deltaTime);
			} else {
				//Debug.Log ("Failed");

			}



			// -- Check
			if (isTargetable) {
				targetFound = true;
				if (isDamageable) {

				}
			} else {
				targetFound = false;
			}

		}


			
	}

	// Entering
	public void Onboarding(){

		// Random Positioning
		int temp = Random.Range(0,4);
		if (temp < 1) {
			transform.position = new Vector3 (-2 * rangeX, 0, Random.Range(-rangeZ, rangeZ));
		} else if (temp < 2) {
			transform.position = new Vector3 (2 * rangeX, 0, Random.Range(-rangeZ, rangeZ));
		} else if (temp < 3) {
			transform.position = new Vector3 (Random.Range(-rangeX, rangeX), 0, -2 * rangeX);
		} else {
			transform.position = new Vector3 (Random.Range(-rangeX, rangeX), 0, 2 * rangeX);
		}

		// Entering
		enteringTargetPos = new Vector3(Random.Range(-rangeX, rangeX), 0, Random.Range(-rangeZ, rangeZ));
		transform.LookAt (enteringTargetPos);
		isEntering = true;
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

		// -- Rotate Update
		RotationUpdate ();

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

		// -- Rotate Update
		RotationUpdate ();
	}





	// TARGET

	bool isTargetable {
		get{
			int i = -80;
			Vector3 rayDirection;
			while (i < 80) {
				rayDirection = Quaternion.AngleAxis (i, transform.up) * transform.forward;

				Ray theRay = new Ray (transform.position, rayDirection);
				RaycastHit hit;

				bool didHitSomething = Physics.Raycast(theRay, out hit, rayTraceSize, targetLayer);

				if (didHitSomething && hit.transform != null){

					// -- Make sure hit object is inside the range
					if(hit.transform.position.x > -rangeX && hit.transform.position.x < rangeX && hit.transform.position.z > -rangeZ && hit.transform.position.z < rangeZ){

						// -- Make sure hit object has target tag
						if (hit.transform.tag == targetTag || hit.transform.parent.transform.tag == targetTag) {

							targetPosition = hit.transform.position;

							// -- Check How Far Away the Enemy
							if (Vector3.Distance (hit.transform.position, transform.position) < damageableRange) {

								// -- Add damages but check if the target class exists in hit or hit's parent
								if (hit.transform.GetComponent<HealthManager> () != null) {
									hit.transform.GetComponent<HealthManager> ().GetDamage (1.0f);
								} else {
									hit.transform.parent.transform.GetComponent<HealthManager> ().GetDamage (1.0f);
								}
							}

							return true;

						}
					}
				} 

				i += 30;
			}

			return false;
		}
	}
}
