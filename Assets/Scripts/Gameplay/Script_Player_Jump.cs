using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//contains jump functionality for the player
public class Script_Player_Jump : MonoBehaviour {
	
	Script_Player playerScript;
	Rigidbody rb;
	
    const float jumpStrength = 4;
    const int jumpCooldown = 5;
    int jumpStep;
    const float superJumpMultiplier = 3;
	const float perpendicularJumpStrength = 12;
	private const float jumpAngle = -0.1f; //used to stop wall jumping
	
	// Use this for initialization
	void Start () {		
		playerScript = this.gameObject.GetComponent<Script_Player>();
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void FixedUpdate() { //for physics interactions
		if(playerScript.getContacts().Any() && !playerScript.isPaused()) { //if list is nonempty
			processJump(playerScript.getContacts());
		}
	}
	
	private void processJump(List<GameObject> contacts) {
		if(jumpStep > 0) {
            jumpStep--;
        }
			
        if((Input.GetKeyDown("space") || Input.GetMouseButtonDown(0)) && jumpStep == 0) { //jump command issued
            //only jump when jumpStep = 0; jump is off cooldown; prevents multiple contact surfaces causing AddForce to be called multiple times

            bool onGround = false; //ball cant jump if the only contact surface is a wall
			bool super = false;
			bool perpend = false;
			Vector3 normalVector = Vector3.zero; //only used for perpendicular panels
            foreach(GameObject obj in contacts) {
                string tag = obj.tag;
                
                if(playerScript.isPhysical(tag)) { //is a physically interactable surface
                    jumpStep = jumpCooldown; 
					Vector3 normal = playerScript.findNormalVector(obj);
                          
                    if(tag == "Bouncy") {
                        super = true;
						
                    } else if(tag == "Perpendicular") {
						perpend = true;
						normalVector += normal;
					}
					
					//checks if the angle between gravity and the surface normal is more than 90 deg
					//i.e. if the ball is on the ground (as opposed to against a wall)
					if(Vector3.Dot(normal.normalized, Physics.gravity.normalized) < jumpAngle) {
						onGround = true;
					}
                }
            }

			bool specialJump = false; //whether a special jump surface is being contacted
			
			if(super && onGround) { //yellow panel is being contacted
				rb.AddForce(-Physics.gravity.normalized * jumpStrength * superJumpMultiplier, ForceMode.Impulse);
				specialJump = true;
			} 
			
			if(perpend) { 	//perpendicular panel is being contacted
				rb.AddForce(normalVector * perpendicularJumpStrength, ForceMode.Impulse);
				specialJump = true;
			}
            
			if(!specialJump && onGround) { //jump normally
				rb.AddForce(-Physics.gravity.normalized * jumpStrength, ForceMode.Impulse);
			}
        }
    }
	
}
