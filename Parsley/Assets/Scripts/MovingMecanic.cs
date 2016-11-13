﻿using UnityEngine;
using System.Collections;

public class MovingMecanic : MonoBehaviour {

	// Game Manager
	GameManager GMgameManager;

	// -- Target Object is Found or Not
	public bool targetFound = false;

	// -- In Motion (Plant doesn't move until motion)
	public bool inMotion = false;



	// -- Motion Range
	public float rangeX ,rangeZ;



	// TARGET
	public Vector3 targetPosition;

	// -- Target Tag
	public string targetTag;

	// -- Target Check
	float rayTraceSize = 2.0f;

	// -- boolean to check if the target is close enough
	bool isDamageable = false;
	float damageableRange = 1.5f;

	// -- Layers
	public LayerMask targetLayer;


	// FLOCKING

	public float flockingSpeed = 0.001f;
	public float flockingAccelerator = 1.0f;
	float flockingRotationSpeed = 1.0f;
	public float flockingRotationAccelerator = 1.0f;
	Vector3 flockingAverageHeading;
	Vector3 flockingAveragePosition;
	Vector3 flockingGoalPos = Vector3.zero;
	// -- how close do they have to in order to be part of group
	public float flockingNeighbourDistance = 2.0f;
	// -- if it's outside of the boundary, the boolean becomes true
	bool flockingTurning = false;


	// Use this for initialization
	void Start () {

		// Grabbing Game Manager CS class
		GMgameManager = GameObject.FindGameObjectWithTag ("GameManager").transform.GetComponent<GameManager>();

		// Area Range
		rangeX = GMgameManager.rangeX;
		rangeZ = GMgameManager.rangeZ;
			
	}
	
	// Update is called once per frame
	void Update () {


		// -- if 2D
		//transform.eulerAngles.z = transform.eulerAngles.x = 0;
		//transform.position.y = 0;


		// MOVE
		// Move only if it is in motion
		if (inMotion) {


			// -- Check if enemy is around
			if (isTargetable) {
				targetFound = true;
			} else {
				targetFound = false;
				flockingGoalPos = GMgameManager.goalPos;
			}

			// -- Check if it's outside of range
			if (rangeX > transform.position.x && -1 * rangeX < transform.position.x && rangeZ > transform.position.z && -1 * rangeZ < transform.position.z)
				flockingTurning = false;
			else
				flockingTurning = true;

			// APPLY MOTION
			if (flockingTurning) {

				// -- If it's outside of range head toward 0 point
				Vector3 dir = Vector3.zero - transform.position;
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (dir), flockingRotationSpeed * flockingRotationAccelerator * Time.deltaTime);
				flockingSpeed = Random.Range (0.5f, 1);
			} else {

				// --If it's inside apply flocking
				if (Random.Range (0, 5) < 1 || targetFound)
					ApplyFlocking ();
			}

			transform.Translate (0, 0, Time.deltaTime * flockingSpeed * flockingAccelerator);

		}

		if (flockingAccelerator > 1.0f)
			flockingAccelerator -= 0.01f;

		if (flockingRotationAccelerator > 1.0f)
			flockingRotationAccelerator -= 0.01f;
	}


	// TARGET

	bool isTargetable {
		get{
			int i = -120;
			Vector3 rayDirection;
			while (i < 120) {
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
									hit.transform.GetComponent<HealthManager> ().GetDamage (0.2f);
								} else {
									hit.transform.parent.transform.GetComponent<HealthManager> ().GetDamage (0.2f);
								}

								// -- What happens to attachiking pose if plant
								if(transform.tag == "Plant"){
									if(transform.GetComponent<Plant> ().lifeBranchAdditionalScaleAtAttack.y < 0.5f)
										transform.GetComponent<Plant> ().lifeBranchAdditionalScaleAtAttack += new Vector3 (0, 0.1f, 0);
								}

								// -- Make the enemy to flocking target 
								flockingGoalPos = hit.transform.position;

								flockingAccelerator = 1.3f;
								flockingRotationAccelerator = 4.0f;
							}

							return true;

						}
					}
				} 

				i += 20;
			}

			return false;
		}
	}



	// FLOCKING
	void ApplyFlocking (){

		GameObject[] gos;
		gos = GMgameManager.allPlants;

		Vector3 vcenter = Vector3.zero;
		Vector3 vavoid = Vector3.zero;
		float gSpeed = 0.1f;

		float dist;

		int groupSize = 0;


		foreach (GameObject go in gos) {

			if (go != this.gameObject) {

				dist = Vector3.Distance(go.transform.position, transform.position);

				// -- If it's inside the neighbor distance
				if (dist <= flockingNeighbourDistance) {
					vcenter += go.transform.position;
					groupSize++;

					// -- If it's too close, avoid each other
					if (dist < 1.0f) {
						vavoid = vavoid + (transform.position - go.transform.position);
					}

					// -- Apply speed to the entire group
					MovingMecanic anotherFlock = go.GetComponent<MovingMecanic> ();
					gSpeed = gSpeed + anotherFlock.flockingSpeed;
				}

			}

		}

		// -- if plants are inside of group (close enough that fish are close)
		if (groupSize > 0) {

			vcenter = vcenter / groupSize + (flockingGoalPos - transform.position);
			flockingSpeed = gSpeed / groupSize;

			Vector3 dir = (vcenter + vavoid) - transform.position;

			if (dir != Vector3.zero)
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (dir), flockingRotationSpeed * Time.deltaTime);

		}


	}
}
