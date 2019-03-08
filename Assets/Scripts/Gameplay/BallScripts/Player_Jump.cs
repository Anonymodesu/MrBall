using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//contains jump functionality for the player
public class Player_Jump {

	private Rigidbody rb;
	
    private const float jumpStrength = 4;
    private const int jumpCooldown = 5;
    private int jumpStep;
    private const float superJumpStrength = 12;
	private const float perpendicularJumpStrength = 12;
	private const float jumpAngle = 91; //used to stop wall jumping

	private bool playAnimations;
	private Script_Player_Loader playerScript;
	private GameObject perpendicularExplosion, perpendicularTrail, superExplosion;

	// Use this for initialization
	public Player_Jump(Script_Player_Loader playerScript) {		
		playAnimations = SettingsManager.DisplayJumpEffects;
		this.playerScript = playerScript;
		rb = playerScript.GetComponent<Rigidbody>();

		Balls ballResources = GameObject.Find("Resources").GetComponent<Balls>();
		perpendicularExplosion = ballResources.PerpendicularExplosion;
		perpendicularTrail = ballResources.PerpendicularTrail;
		superExplosion = ballResources.SuperExplosion;
	}
	
	public void processJump(List<GameObject> contacts) {
		if(jumpStep > 0) {
            jumpStep--;
        }
			
        if(InputManager.getInput().buttonDown(Command.Jump) && jumpStep == 0) { //jump command issued
            //only jump when jumpStep = 0; jump is off cooldown; prevents multiple contact surfaces causing AddForce to be called multiple times

            bool onGround = false; //ball cant jump if the only contact surface is a wall
			bool super = false;
			bool perpend = false;
			bool specialJump = false; //whether a special jump surface is being contacted

			Vector3 normalVector = Vector3.zero; //only used for perpendicular panels
            foreach(GameObject obj in contacts) {
                string tag = obj.tag;
                
                if(Script_Player_Loader.isPhysical(tag)) { //is a physically interactable surface
                    jumpStep = jumpCooldown; 
					Vector3 normal = playerScript.findNormalVector(obj);
                          
                    if(tag == "Bouncy") {
                        super = true;
                        //superExplosionFX(obj);
						
                    } else if(tag == "Perpendicular") {
						perpend = true;
						normalVector += normal;

						if(playAnimations) {
							playerScript.StartCoroutine(perpendicularFX(obj));
						}
					}
					
					//checks if the angle between gravity and the surface normal is more than 90 deg
					//i.e. if the ball is on the ground (as opposed to against a wall)
					Vector3 fakeNormal = playerScript.findFakeNormalVector(obj);
					if(Vector3.Angle(fakeNormal, Physics.gravity.normalized) > jumpAngle) {
						onGround = true;
					}

                }
            }

			
			if(super && onGround) { //yellow panel is being contacted
				Vector3 jumpDirection = -Physics.gravity.normalized;
				cancelVelocity(jumpDirection);
				rb.AddForce(jumpDirection * superJumpStrength, ForceMode.Impulse);
				SoundManager.getInstance().playSoundFX(SoundFX.YellowJump);
				specialJump = true;

				playerScript.updateTrails("Bouncy");
			}
			
			//you can 'jump' on perpendicular panels without a ground
			if(perpend) {
				cancelVelocity(normalVector);
				rb.AddForce(normalVector * perpendicularJumpStrength, ForceMode.Impulse);
				SoundManager.getInstance().playSoundFX(SoundFX.OrangeJump);
				//rb.AddTorque(normalVector, ForceMode.Impulse);
				specialJump = true;

				playerScript.updateTrails("Perpendicular");
			}
            
			if(!specialJump && onGround) { //jump normally
				Vector3 jumpDirection = -Physics.gravity.normalized;
				cancelVelocity(jumpDirection);
				rb.AddForce(jumpDirection * jumpStrength, ForceMode.Impulse);
				SoundManager.getInstance().playSoundFX(SoundFX.NormalJump);
			}

        }
    }

    //cancels out velocities that oppose the jump direction
    //needed when the trigger collider has contacted but the physical collider hasnt yet
    public void cancelVelocity(Vector3 jumpDirection) {
    	if(Vector3.Dot(rb.velocity, jumpDirection) < 0) {
			rb.velocity = Vector3.ProjectOnPlane(rb.velocity, jumpDirection);
		}
    }
	
    //generate cool special effects for orange ramps
	private IEnumerator perpendicularFX(GameObject ramp) {
		Vector3 explosionPos = playerScript.findContactPoint(ramp);
		Quaternion explosionRot = Quaternion.LookRotation(playerScript.findNormalVector(ramp), 
									Random.insideUnitSphere);
		UnityEngine.Object.Instantiate(perpendicularExplosion, explosionPos, explosionRot);

		GameObject trail = UnityEngine.Object.Instantiate(perpendicularTrail);
		while(trail.activeSelf) {
			trail.transform.position = playerScript.transform.position;

			////spark emit in the opposite direction of velocity
			trail.transform.rotation = Quaternion.LookRotation(-rb.velocity + 0.001f * Random.insideUnitSphere); 
			yield return new WaitForFixedUpdate();
		}

		UnityEngine.Object.Destroy(trail);
	}

	 //generate cool special effects for yellow ramps
	private void superExplosionFX(GameObject ramp) {
		Quaternion explosionRot = Quaternion.LookRotation(playerScript.findNormalVector(ramp), 
									Random.insideUnitSphere);
		Vector3 explosionPos = playerScript.findContactPoint(ramp) + explosionRot * (new Vector3(0,0,0.2f));

		UnityEngine.Object.Instantiate(superExplosion, explosionPos, explosionRot);
	}
}
