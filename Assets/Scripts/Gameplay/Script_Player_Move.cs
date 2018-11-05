using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

//process player's movements over panels
public class Script_Player_Move : MonoBehaviour {

	private class Movement {
        public int forward;
        public int right;
        public bool brake;
        
        public Movement(int forward, int right, bool brake) {
            this.forward = forward;
            this.right = right;
            this.brake = brake;
        }
    }
	
	//user input variables
    private const float normalSpeed = 968; //original values were 880/3600
    private const float superSpeed = 3960;
	private const float maxNormalSpeed = 13;
	private const float maxSuperSpeed = 100;
    private const float brakeStrength = 70;
    private const float brakeThreshold = 0.1f; //threshold velocity at which the player will brake to a complete stop
	private const float maxTorque = 60; //maximum rotational speed
    Movement lastInstruction; //last movement instruction being processed; see processNextInstruction()
	
	Script_Game_Camera cameraScript; 
	Rigidbody rb;

	

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		lastInstruction = new Movement(0,0,false);
		rb.maxAngularVelocity = maxTorque;
		
		cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<Script_Game_Camera>();
	}

	public void processMovement(List<GameObject> contacts) {
        float speed = normalSpeed;
		float maxSpeed = maxNormalSpeed;  //maximum speed depends on the type of panel being contacted
        bool onGround = false;
        
        foreach(GameObject surface in contacts) { //process the current surfaces in contact with the player
            
            if(Script_Player.isPhysical(surface.tag)) {
                onGround = true;
            }
            
            switch(surface.tag) {
                case "Fast":
                speed = superSpeed; //fast movement on purple surfaces
				maxSpeed = maxSuperSpeed; //if on fast panel, maxSuperSpeed is used for speed calcs below
                break;
             
            }
        }

        
        if(onGround) { //process player input
                    
            if(lastInstruction.brake) { //currently braking
                if(rb.velocity.magnitude < brakeThreshold) {
                    rb.velocity = Vector3.zero; //complete stop under a certain speed
					
                } else {
                    rb.AddForce(-brakeStrength * rb.velocity * Time.deltaTime); //braking force proportional to current velocity
					rb.AddTorque(-brakeStrength * 0.3f * rb.angularVelocity * Time.deltaTime);
                }
				
				if(rb.angularVelocity.magnitude < brakeThreshold) {
					rb.angularVelocity = Vector3.zero; //stop spinning
				}
                
                
            } else {
                Vector3 forwardDirection = cameraScript.forwardVector();
                Vector3 rightDirection = cameraScript.rightVector();
                //can only move on the x-z plane
                //(distance/frame) * (60s / deltaTime) = distance / min
                Vector3 facingDirection = (rightDirection * lastInstruction.right + forwardDirection * lastInstruction.forward);
				
				Vector3 current = rb.velocity;
				Vector3 max = (rb.velocity + facingDirection * 0.01f).normalized * maxSpeed; //a small value of 0.01f * facingDirection is added to prevent velocityRatio from being 0
				/*
				ratio of current velocity compared to max velocity, using the maxSpeed variable.
				ball is accelerated less the closer it is to the maximum velocity
				ratio < 1 if the ball is being pushed in the same direction as its current velocity
				ratio > 1 if the ball is being pushed in the opposite direction
				*/
				float velocityRatio = (max - current * Vector3.Dot(current.normalized, facingDirection.normalized)).magnitude;
				velocityRatio = (float) Math.Pow(velocityRatio / maxSpeed, 2);
				
				Vector3 force = facingDirection.normalized * speed * velocityRatio * Time.deltaTime;
				rb.AddForce(force);
				//Debug.Log(velocityRatio + " " + rb.velocity.magnitude + "/" + maxSpeed);
				//Debug.Log(rb.angularVelocity.magnitude);
            }
        }
        
    }
	
    public void processNextInstruction() {
        int forward = 0;
        int right = 0;
        bool brake = false;
			
		if(forward == 0) { //sometimes getkeydown/up events are not detected?
			if(Input.GetButton("Forward")) {
				forward = 1;
			} else if(Input.GetButton("Backward")) {
				forward = -1;
			} else {
				forward = 0;
			}
		}
		if(right == 0) {
			if(Input.GetButton("Right")) {
				right = 1;
			} else if(Input.GetButton("Left")) {
				right = -1;
			} else {
				right = 0;
			}
		}
        
        if(Input.GetMouseButton(1)) {  //right click = brake
            forward = 0; //overrides all previous movement instructions
            right = 0;
            brake = true;
        } else { //released brake
            brake = false;
        }
        
		lastInstruction.forward = forward;
		lastInstruction.right = right;
		lastInstruction.brake = brake;
    }
	
	//clear all previous instructions (sometimes keyup triggers are missed due to death)
	public void resetInput() {
		lastInstruction = new Movement(0,0,false);
	}
	
	public float getMaxNormalSpeed() {
		return maxNormalSpeed;
	}

}
