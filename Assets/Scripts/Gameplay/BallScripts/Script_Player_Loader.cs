
﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


//stores variables assigned by inspector, and provides an interface for other classes to access the ball's transform
public class Script_Player_Loader : MonoBehaviour {
	
	//player collider variables
	[SerializeField]
	private SphereCollider triggerCollider; 

	//last encountered checkpoint; change in editor to allow for testing
	[SerializeField]
    private GameObject startPos;

	[SerializeField]
	private GameObject rampImpactEffect;

	[SerializeField]
	public List<Script_Ramp_Animator> animatedRamps;

	private Player_Controller controlScript;

	public int Cubies {
		get { return controlScript.Cubies; }
	}
	public int Deaths {
		get { return controlScript.Deaths; }
	}
	public float TimePassed {
		get { return controlScript.TimePassed; }
	}

	private BallType currentBall;
	public BallType CurrentBall {
		get { return currentBall; }
	}


    // Use this for initialization
    void Start () {
    	loadBall();
    }

    // Update is called once per frame
    void Update () { //REPLACE WITH FIXED UPDATE?    
       controlScript.Update();
    }
	
	void FixedUpdate() { //does not run when Time.timeScale = 0
		controlScript.FixedUpdate();
	}

	public void updateTrails(string tag) {
		controlScript.updateTrails(tag);
	}

	void OnTriggerEnter(Collider other) { 
        controlScript.OnTriggerEnter(other);
    }
    
    void OnTriggerExit(Collider other) {
        controlScript.OnTriggerExit(other);
    }

    void OnTriggerStay(Collider other) {
    	controlScript.OnTriggerStay(other);
    }

    //play a collision sound is the collision is strong enough
    void OnCollisionEnter(Collision collision) {
    	controlScript.OnCollisionEnter(collision);
	}

	private void loadBall() {

		if(SettingsManager.QuickSaveLoaded) {
			currentBall = PlayerManager.getInstance().getQuicksave(SettingsManager.CurrentPlayer).ballUsed;
		} else {
			currentBall = SettingsManager.CurrentBall;
		}

		GetComponent<Renderer>().material = GameObject.Find("Resources").GetComponent<Balls>().getBall(currentBall);

		Player_Trails trailsScript = null;
		Player_Move movementScript = null;
		Player_Jump jumpScript = null;

		//default components for MrBall
		if(SettingsManager.DisplayTrails) {
		    trailsScript = new Enabled_Trails(this);
		} else {
			trailsScript = new Empty_Trails();
		}
		jumpScript = new Player_Jump(this);

		//custom components
		switch(currentBall) {
			case BallType.UsainBowl:
				movementScript = new UsainBowl_Move(this);
				break;

			default: 
				movementScript = new Player_Move(this);
				break;
		}

		switch(currentBall) {
			case BallType.UsainBowl:
				controlScript = new UsainBowl_Controller(this, jumpScript, movementScript, trailsScript,
										startPos, triggerCollider, animatedRamps);
				break;

			default: 
				controlScript = new Player_Controller(this, jumpScript, movementScript, trailsScript,
												startPos, triggerCollider, animatedRamps);
				break;
		}
		
	} 

	public void storeQuickSave() {
		controlScript.storeQuickSave();
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
    	controlScript.resetGravity();
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