using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player_Controller {

	//external components
	protected Script_Player_Loader playerScript;
	protected Player_Move movementScript;
	protected Player_Jump jumpScript;
	protected Player_Trails trailsScript;
	protected OutOfBounds outOfBoundsScript;

	//serialized fields in Script_Player_Loader
	protected SphereCollider triggerCollider;
	private GameObject startPos;
	private List<Script_Ramp_Animator> animatedRamps;

	//collider variables
	private const float minSize = 0.55f;
	private const float maxSize = 1;
	private const float maxSpeed = 22; //maxSpeed under most circumstances
	protected Rigidbody rb;
    protected List<GameObject> contacts; //holds the current ramps in contact with mr.ball's triggercollider

	//collision FX
	private bool displayDust;
	private const float collisionImpactThreshold = 3.5f;
	private const float collisionImpactDamper = 0.16f;
	private const float collisionSoundThreshold = 1;
	private const float collisionSoundDamper = 0.1f;

	//gravity variables
	private Vector3 startGravityDirection; //stores the gravity at the last checkpoint
	private const float gravityStrength = 9.8f;
	public static readonly Vector3 defaultGravityDirection = Vector3.down; //stores gravity loaded at the beginning of each level
	private float gravityEpsilon = 10; //minimum change in gravity to change the coordinate axes

	//UI variables
	private Script_Checkpoint_Animation checkpointAnimation;
	private Script_Game_Menu GUIScript;

	//scoring variables
	protected string player;
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
	public float TimePassed {
		get { return Time.timeSinceLevelLoad - startTime; }
	}

	private readonly GameObject rootObject; //"Rollercoaster"

	private GameObject rampImpactEffect;

	public Player_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, Player_Trails trailsScript, 
							GameObject startPos, SphereCollider triggerCollider, List<Script_Ramp_Animator> animatedRamps) {
		this.playerScript = playerScript;
		this.jumpScript = jumpScript;
		this.movementScript = movementScript;
		this.trailsScript = trailsScript;

		this.startPos = startPos;
		this.triggerCollider = triggerCollider;
		this.animatedRamps = animatedRamps;

		contacts = new List<GameObject>();
		outOfBoundsScript = new OutOfBounds(playerScript.gameObject);
        rb = playerScript.GetComponent<Rigidbody>();     
		
		GUIScript = GameObject.Find("UI").GetComponent<Script_Game_Menu>();
        
        player = SettingsManager.CurrentPlayer;
				
		currentLevel = GameManager.getInstance().getLevel();
		checkpointAnimation = GameObject.Find("CheckpointNotification").GetComponent<Script_Checkpoint_Animation>();

		//display panel was used as a start_pos to take screenshots of levels
		GameObject.Find("displaypanel").SetActive(false);

		rootObject = GameObject.Find("Rollercoaster");

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		} else {
			loadDefaultSettings();
			reset();
		}
		
		displayDust = SettingsManager.DisplayDust;
		rampImpactEffect = GameObject.Find("Resources").GetComponent<Balls>().RampImpactEffect;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(Time.timeScale != 0) {

	        jumpScript.processJump(contacts);
	        trailsScript.processTrails();

	        if(outOfBoundsScript.outOfBounds()) {
	        	die();
	        }
	        
	    }
		movementScript.processNextInstruction(); //process user input regardless of pause state
	}

	public virtual void FixedUpdate() {
		movementScript.processMovement(contacts);
		processCollider();
	}

	public void OnTriggerEnter(Collider other) {
		string tag = other.gameObject.tag;
        
        if(tag == "Cubie") {
			//sometimes a single cubie is collected twice; this might be because the collider rotates and hits the ball a second time before the cubie is destroyed
			other.enabled = false;
			
			other.gameObject.SetActive(false);
            cubies++;
            SoundManager.getInstance().playSoundFX(SoundFX.Cubie);
            
        } else if (Script_Player_Loader.isPhysical(tag)) {
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

	public void OnTriggerExit(Collider other) {
		string tag = other.gameObject.tag;
        
        if(Script_Player_Loader.isPhysical(tag)) {
            contacts.Remove(other.gameObject);
        }
	}

	public void OnTriggerStay(Collider other) {
		//not in OnTriggerEnter() as the same collider has several normals
    	if(other.gameObject.tag == "Gravity") {
			processGravity(other.gameObject);
    	}
	}

	public void OnCollisionEnter(Collision collision) {
		float hitStrength = collision.impulse.magnitude;

		if(hitStrength > collisionSoundThreshold) {
			float volume = hitStrength * collisionSoundDamper;
			SoundManager.getInstance().playSoundFX(SoundFX.Collision, volume);
		}

		if(displayDust && hitStrength > collisionImpactThreshold) {
			GameObject rampImpact = UnityEngine.Object.Instantiate(rampImpactEffect, 
									playerScript.findContactPoint(collision.gameObject), 
									Quaternion.LookRotation(playerScript.findNormalVector(collision.gameObject), 
									UnityEngine.Random.insideUnitSphere));
			rampImpact.transform.localScale = hitStrength * collisionImpactDamper * Vector3.one;
		}
	}

	public void updateTrails(string tag) {
		trailsScript.updateColours(tag);
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
		Vector3 normal = playerScript.findFakeNormalVector(surface);

		if(Vector3.Angle(-Physics.gravity, normal) > gravityEpsilon) { //change in gravity is significant enough
			SoundManager.getInstance().playSoundFX(SoundFX.Gravity);
			updateTrails("Gravity");
		}

		Physics.gravity = -normal.normalized * gravityStrength;
	}
	
    protected virtual void reset() {
        Physics.gravity = startGravityDirection * gravityStrength;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //calculate the point at which to respawn the ball
        Ray ray = new Ray(startPos.transform.position, -startGravityDirection);
        float distance = Math.Abs(Vector3.Dot(startPos.transform.lossyScale, startGravityDirection));
        //cast a from origin and reverse it to obtain the point on the surface, above which the ball spawns
        ray.origin = ray.GetPoint(distance);
        ray.direction = -ray.direction;
        RaycastHit hit;
        if(startPos.GetComponent<Collider>().Raycast(ray, out hit, distance)) {
        	playerScript.transform.position = hit.point - startGravityDirection;

        } else {
        	Debug.Log("could not find collider in reset()");
        	playerScript.transform.position = startPos.transform.position - startGravityDirection;
        }
         //make sure in startPos, player is not colliding with anything!

		
        movementScript.resetInput();
    }
	
	protected void setCheckpoint(GameObject checkPoint, Vector3 gravity) {
		//check if a new checkpoint has been reached
		if(checkPoint != startPos || gravity != startGravityDirection) {

			startPos = checkPoint;
			startGravityDirection = gravity.normalized;
			SoundManager.getInstance().playSoundFX(SoundFX.Checkpoint);
			playerScript.StartCoroutine(checkpointAnimation.animate());
		}
	}
	
	protected virtual void die() {
		reset();
		deaths++;
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

		//load the quick save for the current player
		Quicksave save = PlayerManager.getInstance().getQuicksave(player);

		if(save != null) {
			if(save.level.Equals(currentLevel)) {
				playerScript.transform.position = save.position;
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

				startPos = findObject(save.startPosIDs);
				startTime = -save.time;
				deaths = save.deaths;

				for(int i = 0; i < save.rampAnimationTimes.Count; i++) {
					animatedRamps[i].setNormalisedPlayTime(save.rampAnimationTimes[i]);
				}

				//check whether all ramps have been saved into the quick save
				int numActiveRamps = UnityEngine.Object.FindObjectsOfType<Script_Ramp_Animator>().Length;
				if(numActiveRamps != save.rampAnimationTimes.Count) {
					Debug.Log("only saved " + numActiveRamps + "/" + save.rampAnimationTimes.Count + " ramps.");
				}

			} else {
				Debug.Log("quicksave " + save.level.ToString() + " loaded for " + currentLevel.ToString());
			}
		}
	}

	//called by Script_Game_Menu when prematurely exiting a level
	public virtual Quicksave storeQuickSave() {
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

		Quicksave save = new Quicksave(currentLevel, playerScript.transform.position, rb.velocity, rb.angularVelocity,
										startGravityDirection, Physics.gravity.normalized, collectedCubies, getUniqueID(startPos),
										TimePassed, deaths, rampAnimationTimes, SettingsManager.CurrentBall);


		PlayerManager.getInstance().storeQuicksave(player, save);

		return save;
	}

	public void resetGravity() {
    	Physics.gravity = gravityStrength * defaultGravityDirection;
    }

    //returns a list of sibling indices which can be used to find the target
    protected List<int> getUniqueID(GameObject target) {
    	List<int> parentSiblingIndices = new List<int>();

		Transform current = target.transform;
		while(current.parent != null) {
			parentSiblingIndices.Add(current.GetSiblingIndex());
			current = current.parent;
		}

		return parentSiblingIndices;
    }

    //find the gameobject given its parents' sibling indices
    protected GameObject findObject(List<int> parentSiblingIndices) {
    	Transform current = rootObject.transform;

		for(int i = parentSiblingIndices.Count - 1; i >= 0; i--) {
			current = current.GetChild(parentSiblingIndices[i]);
		}

		return current.gameObject;
    }
}
