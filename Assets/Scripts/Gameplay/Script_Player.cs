using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//organising class for the all the player's functionality ingame
[RequireComponent(typeof(Script_Player_Move))]
[RequireComponent(typeof(Script_OutOfBounds))]
[RequireComponent(typeof(Script_Player_Jump))]
public class Script_Player : MonoBehaviour {
	
	//player collider variables
	[SerializeField]
	private SphereCollider triggerCollider; 
	private const float minSize = 0.55f;
	private const float maxSize = 1;
	private const float maxSpeed = 22; //maxSpeed under most circumstances
	private Rigidbody rb;
    private List<GameObject> contacts; //holds the current ramps in contact with mr.ball's triggercollider
    
	//scripts
	private Script_Player_Move movementScript;
	private Script_Player_Jump jumpScript;
	private Script_OutOfBounds outOfBoundsScript;

	//last encountered checkpoint; change in editor to allow for testing
	[SerializeField]
    private GameObject startPos;

    //pausing
    private bool stopGame;
	private bool pauseable;
	
	//gravity variables
	private Vector3 startGravityDirection; //stores the gravity at the last checkpoint
	private const float gravityStrength = 9.8f;
	private Vector3 defaultGravityDirection = Vector3.down; //stores gravity loaded at the beginning of each level
	private float gravityEpsilon = 10; //minimum change in gravity to change the coordinate axes

	//UI variables
    private Script_Game_Menu GUIScript;

	//scoring variables
	private string player;
    private float startTime;
    int cubies;
    int deaths;
    Level currentLevel; //'const'
	Achievement requirements; //'const'


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
    void Start () {
        contacts = new List<GameObject>();
        
        Cursor.visible = true; 
		movementScript = GetComponent<Script_Player_Move>();
		jumpScript = GetComponent<Script_Player_Jump>();
		outOfBoundsScript = GetComponent<Script_OutOfBounds>();
        rb = GetComponent<Rigidbody>();     

        Time.timeScale = 0;
		
		GUIScript = GameObject.Find("UI").GetComponent<Script_Game_Menu>();
        
        stopGame = true;
        player = SettingsManager.CurrentPlayer;
        startTime = Time.timeSinceLevelLoad;
        cubies = 0;
        deaths = 0;
        
        pauseable = false;
        
        if(startPos == null) {
        	startPos = GameObject.Find("Ramp_Start");
        }
		startGravityDirection = defaultGravityDirection;// // -startPos.transform.up; defaultGravityDirection
        reset();
				
		currentLevel = GameManager.getInstance().getLevel();
		requirements = GameManager.getInstance().getRequirement(currentLevel);
		if(requirements == null) {
			Debug.Log("error parsing achievements");
			requirements = new Achievement(0,0,0,0);
		}

		checkpointAnimation = GameObject.Find("CheckpointNotification").GetComponent<Script_Checkpoint_Animation>();

		//display panel was used as a start_pos to take screenshots of levels
		GameObject.Find("displaypanel").SetActive(false);

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		}
		
		displayDust = SettingsManager.DisplayDust;

		GetComponent<Renderer>().material = GameObject.Find("Resources").GetComponent<Balls>().getBall(SettingsManager.CurrentBall);
    }

    // Update is called once per frame
    void Update () { //REPLACE WITH FIXED UPDATE?    
        if(!stopGame) {
			Cursor.lockState = CursorLockMode.Locked;
            GUIScript.displayProgress(cubies, getTime(), deaths, requirements);

            jumpScript.processJump(contacts);

            if(outOfBoundsScript.outOfBounds()) {
            	die();
            }
            
        } else {
			Cursor.lockState = CursorLockMode.None; 
		}
        
        if(InputManager.getInput().buttonDown(Command.Pause) && pauseable) {
            pause();
        }

		movementScript.processNextInstruction(); //process user input regardless of pause state
    }

	
	void FixedUpdate() { //for physics interactions
		if(!stopGame) {
			processCollider();
			movementScript.processMovement(contacts);
		}
	}

	public void startGame() {
        pauseable = true;
        pause();
        Cursor.visible = false;
		GUIScript.startGame(); //hides the starting interface
    }
	
	public void endGame() {
		if(!contacts.Contains(GameObject.Find("Ramp_Win"))) {
			storeQuickSave(); //don't store the save when exiting from the winning screen
		}

		Physics.gravity = defaultGravityDirection * gravityStrength;
		SceneManager.LoadScene("Scene_Menu");
    }
	
	public void restartGame() {
		Physics.gravity = defaultGravityDirection * gravityStrength;
		SceneManager.LoadScene(currentLevel.ToString());
	}
	
	public void nextLevel() {
		Physics.gravity = defaultGravityDirection * gravityStrength;
		Level nextLevel;
		
		if(currentLevel.substage == Level.numSubstages - 1) { //this is last level of the stage
			nextLevel = new Level(currentLevel.stage + 1, 0);
			
		} else { //there are further levels in this stage
			nextLevel = new Level(currentLevel.stage, currentLevel.substage + 1);
		}
			
		SceneManager.LoadSceneAsync(nextLevel.ToString());
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
                win();
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
			GetComponent<Script_Player_Trails>().updateColours("Gravity");
		}

		Physics.gravity = -normal.normalized * gravityStrength;
	}
	
    private void reset() {
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
	
	private void win() {
        pauseable = false;
        pause();
        PlayerManager.getInstance().deleteQuicksave(player);
		processScoreAchievements();
    }
	
	public float getTime() {
		return (Time.timeSinceLevelLoad - startTime);
	}
		
	//when the level ends, process the final score and any acquired achievements
    private void processScoreAchievements() {
	
		HighScore currentHighScore = new HighScore(player, cubies, deaths, getTime());
			 
		//returns true if theres a new highscore
		if(ScoreManager.getInstance().setHighScore(currentLevel, currentHighScore)) {
			
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, ScoreManager.getInstance().getHighScores(currentLevel));
			
		} else {
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, null);
		}
		
		Achievement newRecord = new Achievement(cubies,deaths, getTime(), HighScore.calculateScore(cubies,deaths, getTime()));
		PlayerManager.getInstance().saveRecord(player, newRecord, currentLevel); //save records to player data txt
		
    }


    private void pause() {
        if(stopGame) {
            GUIScript.hidePauseMenu();
            Time.timeScale = 1;
            stopGame = false;
            Cursor.visible = false;

        } else {
            GUIScript.showPauseMenu();
            Time.timeScale = 0;
            stopGame = true;
            Cursor.visible = true;
        }
        
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
				startTime -= save.time;
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

	private void storeQuickSave() {
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

		Quicksave save = new Quicksave(player, currentLevel, transform.position, rb.velocity, rb.angularVelocity,
										startGravityDirection, Physics.gravity.normalized, collectedCubies, startPos.name,
										getTime(), deaths, rampAnimationTimes);

		PlayerManager.getInstance().storeQuicksave(player, save);
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
