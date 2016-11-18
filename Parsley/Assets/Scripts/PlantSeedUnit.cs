using UnityEngine;
using System.Collections;

public class PlantSeedUnit : MonoBehaviour {

	public GameObject seeder;

	public float currentScale = 0;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {

		if (currentScale < 1.0f)
			currentScale += 0.05f;

		transform.localScale = new Vector3 (currentScale, currentScale, currentScale);
	
	}
}
