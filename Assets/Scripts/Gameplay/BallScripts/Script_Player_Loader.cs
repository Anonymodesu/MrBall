
﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//organising class for the all the player's functionality ingameprivate 
public class Script_Player_Loader : MonoBehaviour {
	
	//player collider variables
	[SerializeField]
	private SphereCollider triggerCollider; 
	private const float minSize = 0.55f;
	private const float maxSize = 1;
	private const float maxSpeed = 22; //maxSpeed under most circumstances
	private Rigidbody rb;
    private List<GameObject> contacts; //holds the current ramps in contact with mr.ball's triggercollider
    
	//scripts
	private Player_Move movementScript;
	private Player_Jump jumpScript;
	private Empty_Trails trailsScript;
	private Script_OutOfBounds outOfBoundsScript;

	//last encountered checkpoint; change in editor to allow for testing
	[SerializeField]
    private GameObject startPos;

	
	//gravity variables
	private Vector3 startGravityDirection; //stores the gravity at the last checkpoint
	private const float gravityStrength = 9.8f;
	public static readonly Vector3 defaultGravityDirection = Vector3.down; //stores gravity loaded at the beginning of each level
	private float gravityEpsilon = 10; //minimum change in gravity to change the coordinate axes

	//UI variables
    private Script_Game_Menu GUIScript;

	//scoring variables
	private string player;
    private float startTime;
    private int cubies;
    private int deaths;
    private Level currentLevel; //'const'

	public int Cubies {
		get { return cubies; }
	}

	public int Deaths {
		get { return deaths; }
	}



	[SerializeField]
	private GameObject rampImpactEffect;
	private bool displayDust;
	private const float collisionImpactThreshold = 3.5f;
	private const float collisionImpactDamper = 0.16f;
	private const float collisionSoundThreshold = 1;
	private const float collisionSoundDamper = 0.1f;


	private Script_Checkpoint_Animation checkpointAnimation;

	[SerializeField]
	public List<Script_Ramp_Animator> animatedRamps;


    // Use this for initialization
    protected virtual void Start () {
    	loadBall();

        contacts = new List<GameObject>();
		outOfBoundsScript = GetComponent<Script_OutOfBounds>();
        rb = GetComponent<Rigidbody>();     
		
		GUIScript = GameObject.Find("UI").GetComponent<Script_Game_Menu>();
        
        player = SettingsManager.CurrentPlayer;
				
		currentLevel = GameManager.getInstance().getLevel();
		checkpointAnimation = GameObject.Find("CheckpointNotification").GetComponent<Script_Checkpoint_Animation>();

		//display panel was used as a start_pos to take screenshots of levels
		GameObject.Find("displaypanel").SetActive(false);

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		} else {
			loadDefaultSettings();
			reset();
		}
		
		displayDust = SettingsManager.DisplayDust;

    }

    // Update is called once per frame
    protected virtual void Update () { //REPLACE WITH FIXED UPDATE?    
        if(Time.timeScale != 0) {

            jumpScript.processJump(contacts);
            trailsScript.processTrails();

            if(outOfBoundsScript.outOfBounds()) {
            	die();
            }
            
        }
		movementScript.processNextInstruction(); //process user input regardless of pause state
    }

	
	protected virtual void FixedUpdate() { //does not run when Time.timeScale = 0
		movementScript.processMovement(contacts);
		processCollider();
	}

	public void updateTrails(string tag) {
		trailsScript.updateColours(tag);
	}

	void OnTriggerEnter(Collider other) { 
        string tag = other.gameObject.tag;
        
        if(tag == "Cubie") {
			//sometimes a single cubie is collected twice; this might be because the collider rotates and hits the ball a second time before the cubie is destroyed
			other.enabled = false;
			
			other.gameObject.SetActive(false);
            cubies++;
            SoundManager.getInstance().playSoundFX(SoundFX.Cubie);
            
        } else if (isPhysical(tag)) {
        	switch(tag) {
        		case "Checkpoint":
                setCheckpoint(other.gameObject, Physics.gravity.normalized);
                break;
                
                case "Lose":
				die();
                break;
                
                case "Win":
                SoundManager.getInstance().playSoundFX(SoundFX.Win);
                GUIScript.processScoreAchievements();
                break;
        	}

			if(!contacts.Contains(other.gameObject)) { //only add an object if it is not already being contacted
				contacts.Add(other.gameObject);	
			}
        }
    }
    
    void OnTriggerExit(Collider other) {
        string tag = other.gameObject.tag;
        
        if(isPhysical(tag)) {
            contacts.Remove(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
    	//not in OnTriggerEnter() as the same collider has several normals
    	if(other.gameObject.tag == "Gravity") {
			processGravity(other.gameObject);
    	}
    }

    //play a collision sound is the collision is strong enough
    void OnCollisionEnter(Collision collision) {
    	float hitStrength = collision.impulse.magnitude;

		if(hitStrength > collisionSoundThreshold) {
			float volume = hitStrength * collisionSoundDamper;
			SoundManager.getInstance().playSoundFX(SoundFX.Collision, volume);
		}

		if(displayDust && hitStrength > collisionImpactThreshold) {
			GameObject rampImpact = Instantiate(rampImpactEffect, findContactPoint(collision.gameObject), 
						Quaternion.LookRotation(findNormalVector(collision.gameObject), UnityEngine.Random.insideUnitSphere));
			rampImpact.transform.localScale = hitStrength * collisionImpactDamper * Vector3.one;
		}
	}

	private void processCollider() { //adjust hitbox of player depending on velocity.
	
		//interpolate between minSize and maxSize depending velocity
		float proportion = rb.velocity.magnitude / maxSpeed;
		if(proportion > 1) {
			proportion = 1;
		}
		triggerCollider.radius = minSize * (1 - proportion) + maxSize * proportion;
	}
	
	//change the gravity to the vector opposing the normal of the surface
	private void processGravity(GameObject surface) {
		Vector3 normal = findFakeNormalVector(surface);

		if(Vector3.Angle(-Physics.gravity, normal) > gravityEpsilon) { //change in gravity is significant enough
			SoundManager.getInstance().playSoundFX(SoundFX.Gravity);
			updateTrails("Gravity");
		}

		Physics.gravity = -normal.normalized * gravityStrength;
	}
	
    protected virtual void reset() {
        Physics.gravity = startGravityDirection * gravityStrength;
		transform.position = startPos.transform.position - startGravityDirection; //make sure in startPos, player is not colliding with anything!
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
		
        movementScript.resetInput();
    }
	
	private void setCheckpoint(GameObject checkPoint, Vector3 gravity) {
		//check if a new checkpoint has been reached
		if(checkPoint != startPos || gravity != startGravityDirection) {
			startPos = checkPoint;
			startGravityDirection = gravity.normalized;
			SoundManager.getInstance().playSoundFX(SoundFX.Checkpoint);
			StartCoroutine(checkpointAnimation.animate());
		}
	}
	
	private void die() {
		reset();
		deaths++;
	}
	
	public float getTime() {
		return (Time.timeSinceLevelLoad - startTime);
	}

	private void loadDefaultSettings() {
		cubies = 0;
		deaths = 0;

		if(startPos == null) {
        	startPos = GameObject.Find("Ramp_Start");
        }

		startGravityDirection = defaultGravityDirection;// // -startPos.transform.up; defaultGravityDirection
		startTime = 0;
	}
		
    private void loadQuickSave() {

		SettingsManager.QuickSaveLoaded = false;

		//load the quick save for the current player
		Quicksave save = PlayerManager.getInstance().getQuicksave(player);

		if(save != null) {
			if(save.level.Equals(currentLevel)) {
				transform.position = save.position;
				rb.velocity = save.velocity;
				rb.angularVelocity = save.angularVelocity;
				startGravityDirection = save.startGravityDirection;
				Physics.gravity = save.currentGravityDirection;
				Physics.gravity *= gravityStrength;

				//if the save.cubies[i] == false, then the cubie was collected
				Transform cubies = GameObject.Find("Cubies").transform;
				for(int i = 0; i < cubies.childCount; i++) {
					if(save.cubies[i]) {
						cubies.GetChild(i).gameObject.SetActive(true);
					} else {
						cubies.GetChild(i).gameObject.SetActive(false);
						this.cubies++;
					}
				}

				startPos = GameObject.Find(save.startPos);
				startTime = -save.time;
				deaths = save.deaths;

				for(int i = 0; i < save.rampAnimationTimes.Count; i++) {
					animatedRamps[i].setNormalisedPlayTime(save.rampAnimationTimes[i]);
				}

				//check whether all ramps have been saved into the quick save
				int numActiveRamps = FindObjectsOfType<Script_Ramp_Animator>().Length;
				if(numActiveRamps != save.rampAnimationTimes.Count) {
					Debug.Log("only saved " + numActiveRamps + "/" + save.rampAnimationTimes.Count + " ramps.");
				}

			} else {
				Debug.Log("quicksave " + save.level.ToString() + " loaded for " + currentLevel.ToString());
			}
		}
		
	}

	//called by Script_Game_Menu when prematurely exiting a level
	public void storeQuickSave() {
		//store whether each cubie has been collected
		List<bool> collectedCubies = new List<bool>();
		Transform cubies = GameObject.Find("Cubies").transform;
		for(int i = 0; i < cubies.childCount; i++) {
			collectedCubies.Add(cubies.GetChild(i).gameObject.activeSelf);
		}

		//store the current amount played per animated ramp
		List<float> rampAnimationTimes = new List<float>();
		foreach(Script_Ramp_Animator ramp in animatedRamps) {
			float animationTime = ramp.getNormalisedPlayTime();
			rampAnimationTimes.Add(animationTime);
		}

		Quicksave save = new Quicksave(currentLevel, transform.position, rb.velocity, rb.angularVelocity,
										startGravityDirection, Physics.gravity.normalized, collectedCubies, startPos.name,
										getTime(), deaths, rampAnimationTimes, SettingsManager.CurrentBall);


		PlayerManager.getInstance().storeQuicksave(player, save);
	}

	private void loadBall() {
		GetComponent<Renderer>().material = GameObject.Find("Resources").GetComponent<Balls>().getBall(SettingsManager.CurrentBall);

		movementScript = new Player_Move(this);
		jumpScript = new Player_Jump(this);

		if(SettingsManager.DisplayTrails) {
			trailsScript = new Player_Trails(this);
		} else {
			trailsScript = new Empty_Trails();
		}

		switch(SettingsManager.CurrentBall) {
			

			case BallType.UsainBowl:
				movementScript = new UsainBowl_Move(this);
				break;
		}
	} 

    public static bool isPhysical(string surface) {        
        return surface == "Fast" || 
               surface == "Bouncy" || 
               surface == "Ground" || 
               surface == "Win" || 
               surface == "Lose" || 
               surface == "Checkpoint" ||
			   surface == "Perpendicular" ||
			   surface == "Gravity";
    }

    public void resetGravity() {
    	Physics.gravity = gravityStrength * defaultGravityDirection;
    }


    //this method assumes the player is a sphere collider
	//returns the contact point of the collision between the player and a ramp
    public Vector3 findContactPoint(GameObject ramp) {
    	return ramp.GetComponent<Collider>().ClosestPoint(transform.position);
    }

	//this method assumes the player is a sphere collider
    //returns the normal vector of the collision between the player and a ramp
	public Vector3 findNormalVector(GameObject ramp) {

		Ray ray = new Ray(transform.position, findContactPoint(ramp) - transform.position);
		RaycastHit hit;

		if(ramp.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity)) {
			return hit.normal;
		}

		//should never happen
		Debug.Log("can't find ramp " + ramp.name + " in findNormalVector()");
		return transform.position - findContactPoint(ramp);
	}

	//used in processGravity and detecting wall jumps, since the above method does some weird stuf otherwise
	public Vector3 findFakeNormalVector(GameObject ramp) {
		Ray ray = new Ray(transform.position, ramp.transform.position - transform.position);
		RaycastHit hit;

		if(ramp.GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity)) {
			return hit.normal;
		}

		//should never happen
		Debug.Log("can't find ramp " + ramp.name + " in findNormalVectorGravity()");
		return transform.position - findContactPoint(ramp);
	}
}