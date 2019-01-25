using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

//contains jump functionality for the player
public class Script_Player_Jump : MonoBehaviour {
	
	#pragma warning disable 0649
	[SerializeField]
	private GameObject perpendicularExplosion, perpendicularTrail, superExplosion;
	#pragma warning restore 0649

	private Rigidbody rb;
	
    private const float jumpStrength = 4;
    private const int jumpCooldown = 5;
    private int jumpStep;
    private const float superJumpStrength = 12;
	private const float perpendicularJumpStrength = 12;
	private const float jumpAngle = 91; //used to stop wall jumping

	private bool playAnimations;

	// Use this for initialization
	void Start () {		
		rb = GetComponent<Rigidbody>();
		playAnimations = SettingsManager.DisplayJumpEffects;
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
                
                if(Script_Player.isPhysical(tag)) { //is a physically interactable surface
                    jumpStep = jumpCooldown; 
					Vector3 normal = GetComponent<Script_Player>().findNormalVector(obj);
                          
                    if(tag == "Bouncy") {
                        super = true;
                        //superExplosionFX(obj);
						
                    } else if(tag == "Perpendicular") {
						perpend = true;
						normalVector += normal;

						if(playAnimations) {
							StartCoroutine(perpendicularFX(obj));
						}
					}
					
					//checks if the angle between gravity and the surface normal is more than 90 deg
					//i.e. if the ball is on the ground (as opposed to against a wall)
					Vector3 fakeNormal = GetComponent<Script_Player>().findFakeNormalVector(obj);
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

				GetComponent<Script_Player_Trails>().updateColours("Bouncy");
			}
			
			//you can 'jump' on perpendicular panels without a ground
			if(perpend) {
				cancelVelocity(normalVector);
				rb.AddForce(normalVector * perpendicularJumpStrength, ForceMode.Impulse);
				SoundManager.getInstance().playSoundFX(SoundFX.OrangeJump);
				//rb.AddTorque(normalVector, ForceMode.Impulse);
				specialJump = true;

				GetComponent<Script_Player_Trails>().updateColours("Perpendicular");
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
    private void cancelVelocity(Vector3 jumpDirection) {
    	if(Vector3.Dot(rb.velocity, jumpDirection) < 0) {
			rb.velocity = Vector3.ProjectOnPlane(rb.velocity, jumpDirection);
		}
    }
	
    //generate cool special effects for orange ramps
	private IEnumerator perpendicularFX(GameObject ramp) {
		Vector3 explosionPos = GetComponent<Script_Player>().findContactPoint(ramp);
		Quaternion explosionRot = Quaternion.LookRotation(GetComponent<Script_Player>().findNormalVector(ramp), 
									Random.insideUnitSphere);
		Instantiate(perpendicularExplosion, explosionPos, explosionRot);

		GameObject trail = Instantiate(perpendicularTrail);
		while(trail.activeSelf) {
			trail.transform.position = transform.position;
			trail.transform.rotation = Quaternion.LookRotation(-rb.velocity); //spark emit in the opposite direction of velocity
			yield return new WaitForFixedUpdate();
		}

		Destroy(trail);
	}

	 //generate cool special effects for yellow ramps
	private void superExplosionFX(GameObject ramp) {
		Quaternion explosionRot = Quaternion.LookRotation(GetComponent<Script_Player>().findNormalVector(ramp), 
									Random.insideUnitSphere);
		Vector3 explosionPos = GetComponent<Script_Player>().findContactPoint(ramp) + explosionRot * (new Vector3(0,0,0.2f));

		Instantiate(superExplosion, explosionPos, explosionRot);
	}
}
