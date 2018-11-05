using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generates speed lines when the player is moving faster than a threshold velocity
public class Script_Player_Trails : MonoBehaviour {
	[SerializeField]
	private GameObject trail;

	private Rigidbody rb;
	private float fadeTime;
	private TrailRenderer mainTrail;
	private TrailRenderer[] secondaryTrails;
	private Vector3[] secondaryPositions;

	private const float threshold = 5;
	private const float width = 1;
	private const float time = 0.5f;

	private const float secondaryThreshold = 10;
	private const float spawnRadius = 0.25f;
	private const float secondaryWidth = 0.4f;
	private const float secondaryTime = 0.3f;
	private const int numSecondaryTrails = 3;
	private const float spawnDelay = 0.3f;


	// Use this for initialization
	void Start () {
		TrailRenderer trailRenderer = trail.GetComponent<TrailRenderer>();
		fadeTime = trailRenderer.time;
		rb = GetComponent<Rigidbody>();

		trailRenderer.time = time;
		trailRenderer.widthMultiplier = width;
		mainTrail = Instantiate(trail).GetComponent<TrailRenderer>();
		mainTrail.emitting = false;

		//secondary trails are thinner and expire faster
		trailRenderer.widthMultiplier = secondaryWidth; 
		trailRenderer.time = secondaryTime;

		secondaryTrails = new TrailRenderer[numSecondaryTrails];
		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryTrails[i] = Instantiate(trail).GetComponent<TrailRenderer>();
			secondaryTrails[i].emitting = false;
		}

		//initial reference positions for secondary trails
		secondaryPositions = new Vector3[numSecondaryTrails];
		for(int i = 0; i < numSecondaryTrails; i++) {
			float degrees = Mathf.Deg2Rad * i * 120;
			secondaryPositions[i] = spawnRadius * (new Vector3(Mathf.Cos(degrees), Mathf.Sin(degrees), 0));
		}
	}
	
	// Toggle trail visibility when above/below velocity thresholds
	void Update () {

		mainTrail.transform.position = transform.position;
		if(rb.velocity.magnitude > threshold) {
			mainTrail.emitting = true;
		} else {
			mainTrail.emitting = false;
		}

		setSecondaryPositions();
		if(rb.velocity.magnitude > secondaryThreshold) {
			StartCoroutine(spawnSecondaryEmission());
		} else {
			despawnSecondaryEmission();
		}
	}

	//periodically spawn secondary trails
	private IEnumerator spawnSecondaryEmission() {
		foreach(TrailRenderer trail in secondaryTrails) {
			trail.emitting = true;
			yield return new WaitForSeconds(spawnDelay);
		}
	}

	//immediately stop emissions
	private void despawnSecondaryEmission() {
		foreach(TrailRenderer trail in secondaryTrails) {
			trail.emitting = false;
		}
	}

	private void setSecondaryPositions() {
		//the rotation that will make the emission plane perpendicular to velocity
		Quaternion velocityRotation = Quaternion.FromToRotation(Vector3.forward, rb.velocity);

		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryTrails[i].transform.position = transform.position + velocityRotation * secondaryPositions[i];
		}
	}

	//sets the spawn offset of the secondary trails
	//ensures that the trail spawns in the opposite direction of the ball's current velocity
	//CURRENTLY UNUSED
	private Vector3 offsetPos() {
		Vector3 offset = spawnRadius * Random.insideUnitSphere;
		if(Vector3.Dot(offset, rb.velocity) < 0) {
			offset = -offset;
		}

		return offset;
	}
}
