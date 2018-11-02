﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//organising class for the all the player's functionality ingame
public class Script_Player : MonoBehaviour {
	
	//player collider variables
	public SphereCollider triggerCollider; 
	private const float minSize = 0.55f;
	private const float maxSize = 1;
	private const float maxSpeed = 22; //maxSpeed under most circumstances
    
	//misc
	private bool stopGame;
	Script_Game_Camera cameraScript; 
	Script_Player_Move movementScript;
    Rigidbody rb;
    List<GameObject> contacts; //holds the current ramps in contact with mr.ball
	private bool pauseable;
    GameObject startPos;
	
	//gravity variables
	Vector3 startGravityDirection; //stores the gravity at the last checkpoint
	private const float gravityStrength = 9.8f;
	private Vector3 defaultGravityDirection = Vector3.down; //stores gravity loaded at the beginning of each level
	private float gravityEpsilon = 0.05f; //minimum change in gravity to change the coordinate axes

	//UI variables
    private Script_Game_Menu GUIScript;

	//scoring variables
    private float startTime;
    int cubies;
    int deaths;
    Level currentLevel; //'const'
	Achievement requirements; //'const'


    // Use this for initialization
    void Start () {
        contacts = new List<GameObject>();
        
        Cursor.visible = true; 
		cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<Script_Game_Camera>();
		movementScript = this.gameObject.GetComponent<Script_Player_Move>();
        rb = GetComponent<Rigidbody>();     

        Time.timeScale = 0;
		
		GUIScript = GameObject.Find("UI").GetComponent<Script_Game_Menu>();
        
        stopGame = true;
        startTime = 0;
        cubies = 0;
        deaths = 0;
        
        pauseable = false;
        
        startPos = GameObject.Find("Ramp_Start");
		startGravityDirection = defaultGravityDirection; //startPos.transform.rotation * Vector3.down
        reset();
				
		currentLevel = GameManager.getInstance().getLevel();
		requirements = AchievementManager.getInstance().getRequirement(currentLevel);
		if(requirements == null) {
			Debug.Log("error parsing achievements");
			requirements = new Achievement(0,0,0,0);
		}
		
    }
    
    // Update is called once per frame
    void Update () { //REPLACE WITH FIXED UPDATE?    
    
        
        if(!stopGame) {
			Cursor.lockState = CursorLockMode.Locked;
            GUIScript.displayProgress(cubies, getTime(), deaths, requirements);
            //Debug.Log(1.0f / Time.smoothDeltaTime);
            
        } else {
			Cursor.lockState = CursorLockMode.None; 
		}
        
        if(Input.GetKeyDown(KeyCode.Escape) && pauseable) {
            pause();
        }

    }
	
	void FixedUpdate() { //for physics interactions
		if(contacts.Any() && !stopGame) { //if list is nonempty
			processCollider();
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
		Physics.gravity = defaultGravityDirection * gravityStrength;//this is needed as the call to reset() in Start() occurs after Script_Game_Camera accesses Physics.gravity
        SceneManager.LoadSceneAsync("Scene_Menu");
    }
	
	public void restartGame() {
		Physics.gravity = defaultGravityDirection * gravityStrength; 
		SceneManager.LoadSceneAsync(currentLevel.ToString());
	}
	
	public void nextLevel() {
		Level nextLevel;
		
		if(currentLevel.substage == GameManager.numSubstages - 1) { //this is last level of the stage
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
            
        } else if (isPhysical(tag)) {
			
			if(!contacts.Contains(other.gameObject)) { //only add an object if is not being contacted
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
    
	
	//change the gravity to the vector opposing the normal of the surface
	public void processGravity(GameObject surface) {
		Vector3 normal = findNormalVector(surface);
		
		if((Physics.gravity.normalized + normal.normalized).magnitude > gravityEpsilon) { //change in gravity is significant enough
			Physics.gravity = -normal.normalized * gravityStrength;
			cameraScript.updateAxes(); //elicit a change in camera perspective
		}
	}
	
	//returns the normal vector if a ray was sent from the ball to the target
	public Vector3 findNormalVector(GameObject target) {
		
		//ray originating from the ball, ending at the physical panel.
		Ray ray = new Ray(this.transform.position, target.transform.position - this.transform.position);
		RaycastHit[] hits = Physics.RaycastAll(ray, maxSize * 500);
					
		foreach(RaycastHit hit in hits) { //look for the game object that the ball is in contact with
			if(hit.collider.gameObject == target) {
				return hit.normal;
			}
		}
		
		Debug.Log("ray cast didnt encounter required collider"); //should never happen
		return Vector3.zero;
	}
    
    public void reset() {
        Physics.gravity = startGravityDirection * gravityStrength;
		transform.position = startPos.transform.position - startGravityDirection; //make sure in startPos, player is not colliding with anything!
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
		
		cameraScript.updateAxes();
        movementScript.resetInput();
        //contacts.Clear(); //sometimes deaths are counted twice
    }
	
	public void setCheckpoint(GameObject checkPoint, Vector3 gravity) {
		startPos = checkPoint;
		startGravityDirection = gravity.normalized;
	}
	
	public void die() {
		reset();
		deaths++;
	}
	
	public void win() {
        pauseable = false;
        pause();
		processScoreAchievements();
    }
	
	public float getTime() {
		return (Time.time - startTime);
	}
    
    public bool isPhysical(string surface) {        
        return surface == "Fast" || 
               surface == "Bouncy" || 
               surface == "Ground" || 
               surface == "Win" || 
               surface == "Lose" || 
               surface == "Checkpoint" ||
			   surface == "Perpendicular" ||
			   surface == "Gravity";
    }
	
	public List<GameObject> getContacts() {
		return contacts;
	}
	
	public bool isPaused() {
		return stopGame;
	}
    
	
	
	private void processCollider() { //adjust hitbox of player depending on velocity.
	
		//interpolate between minSize and maxSize depending velocity
		float proportion = rb.velocity.magnitude / maxSpeed;
		if(proportion > 1) {
			proportion = 1;
		}
		triggerCollider.radius = minSize * (1 - proportion) + maxSize * proportion;
		
		
	}
		
	//when the level ends, process the final score and any acquired achievements
    private void processScoreAchievements() {
	
		HighScore currentHighScore = new HighScore(PlayerPrefs.GetString("name", "New Player"), cubies, deaths, (int) getTime());
			 
		//returns true if theres a new highscore
		if(ScoreManager.getInstance().setHighScore(currentLevel, currentHighScore)) {
			
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, ScoreManager.getInstance().getHighScores(currentLevel));
			
		} else {
			GUIScript.displayScoreAchievements(cubies, deaths, getTime(), requirements, null);
		}
		
		Achievement current = new Achievement(cubies,deaths,(int)getTime(), HighScore.calculateScore(cubies,deaths,(int)getTime()));
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
	
	
}