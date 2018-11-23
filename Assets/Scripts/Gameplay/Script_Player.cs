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
[RequireComponent(typeof(Script_Player_Trails))]
public class Script_Player : MonoBehaviour {
	
	//player collider variables
	[SerializeField]
	private SphereCollider triggerCollider; 
	private const float minSize = 0.55f;
	private const float maxSize = 1;
	private const float maxSpeed = 22; //maxSpeed under most circumstances
	private Rigidbody rb;
    private List<GameObject> contacts; //holds the current ramps in contact with mr.ball
    
	//scripts
	private Script_Game_Camera cameraScript; 
	private Script_Player_Move movementScript;
	private Script_Player_Jump jumpScript;
	private Script_OutOfBounds outOfBoundsScript;

	//last encountered checkpoint
    private GameObject startPos;

    //pausing
    private bool stopGame;
	private bool pauseable;
	
	//gravity variables
	private Vector3 startGravityDirection; //stores the gravity at the last checkpoint
	private const float gravityStrength = 9.8f;
	private Vector3 defaultGravityDirection = Vector3.down; //stores gravity loaded at the beginning of each level
	private float gravityEpsilon = 0.01f; //minimum change in gravity to change the coordinate axes

	//UI variables
    private Script_Game_Menu GUIScript;

	//scoring variables
    private float startTime;
    int cubies;
    int deaths;
    Level currentLevel; //'const'
	Achievement requirements; //'const'

	private float collisionSoundThreshold = 1;
	private float collisionSoundDamper = 10;

	private Script_Checkpoint_Animation checkpointAnimation;

    // Use this for initialization
    void Start () {
        contacts = new List<GameObject>();
        
        Cursor.visible = true; 
		cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<Script_Game_Camera>();
		movementScript = GetComponent<Script_Player_Move>();
		jumpScript = GetComponent<Script_Player_Jump>();
		outOfBoundsScript = GetComponent<Script_OutOfBounds>();
        rb = GetComponent<Rigidbody>();     

        Time.timeScale = 0;
		
		GUIScript = GameObject.Find("UI").GetComponent<Script_Game_Menu>();
        
        stopGame = true;
        startTime = 0;
        cubies = 0;
        deaths = 0;
        
        pauseable = false;
        
        startPos = GameObject.Find("Ramp_Checkpoint");
		startGravityDirection = defaultGravityDirection;// // -startPos.transform.up;
        reset();
				
		currentLevel = GameManager.getInstance().getLevel();
		requirements = AchievementManager.getInstance().getRequirement(currentLevel);
		if(requirements == null) {
			Debug.Log("error parsing achievements");
			requirements = new Achievement(0,0,0,0);
		}

		checkpointAnimation = GameObject.Find("CheckpointNotification").GetComponent<Script_Checkpoint_Animation>();
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
        
        if(Input.GetButtonDown("Cancel") && pauseable) {
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
        startTime = Time.time;
        Cursor.visible = false;
		GUIScript.startGame(); //hides the starting interface
    }
	
	public void endGame() {
		Physics.gravity = defaultGravityDirection * gravityStrength;
		SceneManager.LoadSceneAsync("Scene_Menu");
		Time.timeScale = 1;
    }
	
	public void restartGame() {
		Physics.gravity = defaultGravityDirection * gravityStrength; 
		SceneManager.LoadSceneAsync(currentLevel.ToString());
	}
	
	public void nextLevel() {
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
			
			Destroy(other.gameObject);
            cubies++;
            SoundManager.getInstance().playSoundFX(SoundManager.SoundFX.Cubie);
            
        } else if (isPhysical(tag)) {
        	switch(tag) {
        		case "Checkpoint":
                setCheckpoint(other.gameObject, Physics.gravity.normalized);
                break;
                
                case "Lose":
				die();
                break;
                
                case "Win":
                SoundManager.getInstance().playSoundFX(SoundManager.SoundFX.Win);
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
			float volume = hitStrength / collisionSoundDamper;
			SoundManager.getInstance().playSoundFX(SoundManager.SoundFX.Collision, volume);
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
		Vector3 normal = findNormalVector(surface, this.gameObject);
		
		if((Physics.gravity.normalized + normal.normalized).magnitude > gravityEpsilon) { //change in gravity is significant enough
			Physics.gravity = -normal.normalized * gravityStrength;
			cameraScript.updateAxes(); //elicit a change in camera perspective
		}
	}
	
    private void reset() {
        Physics.gravity = startGravityDirection * gravityStrength;
		transform.position = startPos.transform.position - startGravityDirection; //make sure in startPos, player is not colliding with anything!
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
		
		cameraScript.updateAxes();
        movementScript.resetInput();
    }
	
	private void setCheckpoint(GameObject checkPoint, Vector3 gravity) {
		//check if a new checkpoint has been reached
		if(checkPoint != startPos || gravity != startGravityDirection) {
			startPos = checkPoint;
			startGravityDirection = gravity.normalized;
			SoundManager.getInstance().playSoundFX(SoundManager.SoundFX.Checkpoint);
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
		processScoreAchievements();
    }
	
	public float getTime() {
		return (Time.time - startTime);
	}
		
	//when the level ends, process the final score and any acquired achievements
    private void processScoreAchievements() {
	
		HighScore currentHighScore = new HighScore(PlayerPrefs.GetString("name", "New Player"), cubies, deaths, getTime());
			 
		//returns true if theres a new highscore
		if(ScoreManager.getInstance().setHighScore(currentLevel, currentHighScore)) {
			
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, ScoreManager.getInstance().getHighScores(currentLevel));
			
		} else {
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, null);
		}
		
		Achievement current = new Achievement(cubies,deaths, getTime(), HighScore.calculateScore(cubies,deaths, getTime()));
		AchievementManager.getInstance().saveAchievement(PlayerPrefs.GetString("name", "New Player"), current, currentLevel); //save records to player data txt
		
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

    //returns the normal vector if a ray was sent from the ball to the target
	public static Vector3 findNormalVector(GameObject target, GameObject source) {
		
		//ray originating from the ball, ending at the physical panel.
		Ray ray = new Ray(source.transform.position, target.transform.position - source.transform.position);
		RaycastHit[] hits = Physics.RaycastAll(ray, maxSize * 500);
					
		foreach(RaycastHit hit in hits) { //look for the game object that the ball is in contact with
			if(hit.collider.gameObject == target) {
				return hit.normal;
			}
		}
		
		Debug.Log("ray cast didnt encounter required collider"); //should never happen
		return Vector3.zero;
	}
}
