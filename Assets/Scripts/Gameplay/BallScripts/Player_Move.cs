using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

//process player's movements over panels
public class Player_Move {

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
	
	//user input variables; original speed values were 880/3600
	//protected variables are used by usain bowl
    protected virtual float normalSpeed { get { return 106.48f; } }
    protected const float superSpeed = 435.6f;
	protected virtual float maxNormalSpeed { get { return 13; } }
	protected const float maxSuperSpeed = 100;
    private const float brakeStrength = 20;
	private const float maxTorque = 60; //maximum rotational speed
    private Movement lastInstruction; //last movement instruction being processed; see processNextInstruction()
	
	private Script_Game_Camera cameraScript; 
	private Rigidbody rb;
	protected Script_Player_Loader playerScript;
	

	// Use this for initialization
	public Player_Move(Script_Player_Loader playerScript) {
		lastInstruction = new Movement(0,0,false);
		
		cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<Script_Game_Camera>();
		this.playerScript = playerScript;
		rb = playerScript.GetComponent<Rigidbody>();

		rb.maxAngularVelocity = maxTorque;
	}

	public void processMovement(List<GameObject> contacts) {

        float speed = normalSpeed;
		float maxSpeed = maxNormalSpeed;  //maximum speed depends on the type of panel being contacted

		//torque is applied regardless of whether mr ball is on the ground
		//force is only applied on ground
		bool onGround = false;
        
        foreach(GameObject surface in contacts) { //process the current surfaces in contact with the player

        	if(Script_Player_Loader.isPhysical(surface.tag)) {
        		onGround = true;
        	}

            switch(surface.tag) {
                case "Fast":
                speed = superSpeed; //fast movement on purple surfaces
				maxSpeed = maxSuperSpeed; //if on fast panel, maxSuperSpeed is used for speed calcs below

				playerScript.updateTrails("Fast");
                break;
             
            }
        }
                    
        if(lastInstruction.brake) { //currently braking
  
        	//braking force proportional to current velocity
        	if(onGround) {
        		rb.AddForce(-brakeStrength * rb.velocity * Time.deltaTime);
        		rb.AddTorque(-brakeStrength * rb.angularVelocity * Time.deltaTime);
        	} else {
        		//brake at half the strength
        		rb.AddTorque(-brakeStrength * rb.angularVelocity * Time.deltaTime / 2);
        	}

        } else {
            Vector3 forwardDirection = cameraScript.forwardVector();
            Vector3 rightDirection = cameraScript.rightVector();
            Vector3 movingDirection = (rightDirection * lastInstruction.right + forwardDirection * lastInstruction.forward);
            Vector3 torqueDirection = (rightDirection * lastInstruction.forward + forwardDirection * -lastInstruction.right);

			
			Vector3 current = rb.velocity;
			Vector3 max = (rb.velocity + movingDirection * 0.01f).normalized * maxSpeed; //a small value of 0.01f * movingDirection is added to prevent velocityRatio from being 0
			/*
			ratio of current velocity compared to max velocity, using the maxSpeed variable.
			ball is accelerated less the closer it is to the maximum velocity
			ratio < 1 if the ball is being pushed in the same direction as its current velocity
			ratio > 1 if the ball is being pushed in the opposite direction
			*/
			float velocityRatio = (max - current * Vector3.Dot(current.normalized, movingDirection.normalized)).magnitude;
			velocityRatio = (float) Math.Pow(velocityRatio / maxSpeed, 2);
			
			Vector3 torque = torqueDirection.normalized * speed * velocityRatio * Time.deltaTime;

			if(onGround) {
				Vector3 force = 4 * movingDirection.normalized * speed * velocityRatio * Time.deltaTime;
				rb.AddForce(force);
				rb.AddTorque(torque);

			} else { //spin at half the strength in the air
				rb.AddTorque(torque / 2);
			}



			//Debug.Log(velocityRatio + " " + rb.velocity.magnitude + "/" + maxSpeed);
        }
        
        
    }
	
    public virtual void processNextInstruction() {
        int forward = 0;
        int right = 0;
        bool brake = false;
        
        //braking overrides all other movement instructions
        if(InputManager.getInput().buttonDown(Command.Brake)) { 
            brake = true;

        } else { //released brake

        	//able to process other movement instructions
			if(InputManager.getInput().buttonDown(Command.Forward)) {
				forward = 1;
			} else if(InputManager.getInput().buttonDown(Command.Backward)) {
				forward = -1;
			}

			if(InputManager.getInput().buttonDown(Command.Right)) {
				right = 1;
			} else if(InputManager.getInput().buttonDown(Command.Left)) {
				right = -1;
			}
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
