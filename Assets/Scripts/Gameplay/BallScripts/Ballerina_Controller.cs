using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballerina_Controller : Player_Controller {

	private const float counterForce = 500;
	private const float maxSpin = 20;

	private bool spinning;
	private Ballerina_Trails ballerinaTrails;

	public Ballerina_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		spinning = false;
		ballerinaTrails = (Ballerina_Trails) trailsScript;
	}

	public override void Update() {
		base.Update();

		if(InputManager.getInput().buttonDown(Command.Special)) {
			spinning = true;
			ballerinaTrails.startBallerinaTrails();

		} else if (InputManager.getInput().buttonUp(Command.Special)) {
			spinning = false;
			ballerinaTrails.stopBallerinaTrails();
		}

	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if(spinning) {
			Vector3 force = -Physics.gravity.normalized * counterForce * Time.fixedDeltaTime;
			rb.AddForce(force);

			if(rb.angularVelocity.magnitude < maxSpin) {
				rb.AddTorque(force / 10);
			}
		}
	}
}
