using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//contains jump functionality for the player
public class Script_Player_Jump : MonoBehaviour {
	
	private Rigidbody rb;
	
    private const float jumpStrength = 4;
    private const int jumpCooldown = 5;
    private int jumpStep;
    private const float superJumpMultiplier = 3;
	private const float perpendicularJumpStrength = 12;
	private const float jumpAngle = -0.1f; //used to stop wall jumping
	
	// Use this for initialization
	void Start () {		
		rb = GetComponent<Rigidbody>();
	}
	
	public void processJump(List<GameObject> contacts) {
		if(jumpStep > 0) {
            jumpStep--;
        }
			
        if((Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1")) && jumpStep == 0) { //jump command issued
            //only jump when jumpStep = 0; jump is off cooldown; prevents multiple contact surfaces causing AddForce to be called multiple times

            bool onGround = false; //ball cant jump if the only contact surface is a wall
			bool super = false;
			bool perpend = false;
			bool specialJump = false; //whether a special jump surface is being contacted

			Vector3 normalVector = Vector3.zero; //only used for perpendicular panels
            foreach(GameObject obj in contacts) {
                string tag = obj.tag;
                
                if(Script_Player.isPhysical(tag)) { //is a physically interactable surface
                    jumpStep = jumpCooldown; 
					Vector3 normal = Script_Player.findNormalVector(obj, this.gameObject);
                          
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

			
			if(super && onGround) { //yellow panel is being contacted
				rb.AddForce(-Physics.gravity.normalized * jumpStrength * superJumpMultiplier, ForceMode.Impulse);
				specialJump = true;
			} 
			
			if(perpend) { 	//perpendicular panel is being contacted
				rb.AddForce(normalVector * perpendicularJumpStrength, ForceMode.Impulse);
				//rb.AddTorque(normalVector, ForceMode.Impulse);
				specialJump = true;
			}
            
			if(!specialJump && onGround) { //jump normally
				rb.AddForce(-Physics.gravity.normalized * jumpStrength, ForceMode.Impulse);
			}
        }
    }
	
}
