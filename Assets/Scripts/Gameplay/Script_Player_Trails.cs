using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generates speed lines when the player is moving faster than a threshold velocity
public class Script_Player_Trails : MonoBehaviour {
	[SerializeField]
	private GameObject trail;

	private Rigidbody rb;
	private TrailRenderer mainTrail; //child of the player
	private TrailRenderer[] secondaryTrails; //do not have parents
	private Vector3[] secondaryPositions;
	private float previousSecondarySpin; //keeps track of how much the secondary trails have spun

	//trail colours vary depending on ramp interactions
	//actual trail colours lag a bit behind these
	private Color mainColour;
	private Color[] secondaryColours;
	private float colourSpawnDelay; //prevents changed colours from being overriden too quickly
	private HashSet<string> loadedColours; //prevents fast ramps from overriding other colours

	private const float threshold = 5;
	private const float width = 1;
	private const float time = 0.5f;

	private const float secondaryThreshold = 9;
	private const float spawnRadius = 0.25f;
	private const float secondaryWidth = 0.4f;
	private const float secondaryTime = 0.3f;
	private const int numSecondaryTrails = 4;
	private const float spawnDelay = 0.3f;
	private const float secondarySpinSpeed = 0.03f;

	private const float colourChangeSpeed = 0.1f; //how quickly colours change to special colours
	private const float colourRevertSpeed = 0.01f; //how quickly colours revert back to white
	private static readonly Color fastColour = new Color(255f/255, 0f/255, 255f/255, 1);
	private static readonly Color perpendicularColour = new Color(255f/255, 0f/255, 0f/255, 1);
	private static readonly Color bouncyColour = new Color(255/255, 255f/255, 0f/255, 1);


	// Use this for initialization
	void Start () {

		//get trails settings
		if(!SettingsManager.DisplayTrails) {
			Destroy(this);
			return;
		}

		TrailRenderer trailRenderer = trail.GetComponent<TrailRenderer>();
		rb = GetComponent<Rigidbody>();

		//main trail is a child of the player game object
		trailRenderer.time = time;
		trailRenderer.widthMultiplier = width;
		mainTrail = Instantiate(trail, transform.position, Quaternion.identity, transform).GetComponent<TrailRenderer>();
		mainTrail.emitting = false;

		//secondary trails are thinner and expire faster
		trailRenderer.widthMultiplier = secondaryWidth; 
		trailRenderer.time = secondaryTime;

		//initial reference positions for secondary trails
		secondaryPositions = new Vector3[numSecondaryTrails];
		for(int i = 0; i < numSecondaryTrails; i++) {
			float degrees = Mathf.Deg2Rad * i * (360 / numSecondaryTrails);
			secondaryPositions[i] = spawnRadius * (new Vector3(Mathf.Cos(degrees), Mathf.Sin(degrees), 0));
		}

		secondaryTrails = new TrailRenderer[numSecondaryTrails];
		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryTrails[i] = Instantiate(trail).GetComponent<TrailRenderer>();
			secondaryTrails[i].emitting = false;
		}

		previousSecondarySpin = 0;

		mainColour = Color.white;
		secondaryColours = new Color[numSecondaryTrails];
		for(int i = 0 ; i < numSecondaryTrails; i++) {
			secondaryColours[i] = Color.white;
		}
		colourSpawnDelay = 0;
		loadedColours = new HashSet<string>();
	}
	
	// Toggle trail visibility when above/below velocity thresholds
	void Update () {

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
		processColours();
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

	//makes the trails spin on the plane perpendicular to velocity 
	private void setSecondaryPositions() {
		//the rotation that will make the emission plane perpendicular to velocity
		Quaternion velocityRotation = Quaternion.FromToRotation(Vector3.forward, rb.velocity);

		//magnitude of angle of spin since the last frame
		//scaled by current velocity and how much the ball is spinning around its velocity vector
		float spin = Vector3.Project(rb.angularVelocity, rb.velocity).magnitude * rb.velocity.magnitude * secondarySpinSpeed;

		Quaternion spinRotation = Quaternion.AngleAxis(spin + previousSecondarySpin, rb.velocity);

		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryTrails[i].transform.position = transform.position + spinRotation * velocityRotation * secondaryPositions[i];
		}

		previousSecondarySpin = (previousSecondarySpin + spin) % 360f;
	}

	public void updateColours(string tag) {
		if(!loadedColours.Contains(tag)) {

			switch(tag) {
				case "Bouncy":
					StartCoroutine(setColour(bouncyColour, tag));
					colourSpawnDelay = spawnDelay;
					break;
				case "Fast":
					StartCoroutine(setColour(fastColour, tag)); //fast ramps often override special jump colours
					break;
				case "Perpendicular":
					StartCoroutine(setColour(perpendicularColour, tag));
					colourSpawnDelay = spawnDelay;
					break;

				default: //should never happen
					Debug.Log("Script_Player_Trails received tag " + tag + " when updating colours");
					break;
			}

			loadedColours.Add(tag); //currently processing the colour for ramp with this tag
		}

	}

	//staggers new colour settings by spawnDelay
	private IEnumerator setColour(Color colour, string tag) {

		//delay processing if another colour recently loaded
		yield return new WaitForSeconds(colourSpawnDelay);

		mainColour = colour;

		for(int i = 0; i < numSecondaryTrails; i++) {
			yield return new WaitForSeconds(spawnDelay);
			secondaryColours[i] = colour;
		}

		colourSpawnDelay = 0;

		loadedColours.Remove(tag); //completed processing the colour
	}

	private void processColours() {
		float speed = Time.deltaTime * 60;

		//modify actual trail colours
		mainTrail.startColor = Color.Lerp(mainTrail.startColor, mainColour, colourChangeSpeed * speed);
		mainTrail.endColor = Color.Lerp(mainTrail.endColor, mainColour, colourChangeSpeed * speed);
		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryTrails[i].startColor = Color.Lerp(
				secondaryTrails[i].startColor, secondaryColours[i], colourChangeSpeed * speed);
			secondaryTrails[i].endColor = Color.Lerp(
				secondaryTrails[i].endColor, secondaryColours[i], colourChangeSpeed * speed);
		}

		//revert target trail colours back to white
		mainColour = Color.Lerp(mainColour, Color.white, colourRevertSpeed * speed); 
		for(int i = 0; i < numSecondaryTrails; i++) {
			secondaryColours[i] = Color.Lerp(secondaryColours[i], Color.white, colourRevertSpeed * speed);
		}

	}

	private void setColour() {

	}


}
