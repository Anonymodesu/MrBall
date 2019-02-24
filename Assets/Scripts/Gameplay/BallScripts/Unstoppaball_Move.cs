using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unstoppaball_Move : Player_Move {

	private const float velocityThreshold = 2;
	private const float unstopForce = 130;
	private const float unstopTorque = 0.25f * unstopForce;

	public Unstoppaball_Move(Script_Player_Loader playerScript) : base(playerScript) {
	}

	public override void processMovement(List<GameObject> contacts) {
		base.processMovement(contacts);

		if(contacts.Count > 0) {
			Debug.Log(rb.velocity.magnitude);
			Vector3 lateralVelocity = Vector3.ProjectOnPlane(rb.velocity, Physics.gravity);

			//accelerate ball if its moving too slowly
			if(lateralVelocity.magnitude < velocityThreshold) {

				Vector3 force;
				if(lateralVelocity == Vector3.zero) { //not moving laterally; apply random force
					force = Vector3.ProjectOnPlane(Random.insideUnitSphere, Physics.gravity);
				} else {
					force = lateralVelocity; //augment current velocity
				}
				force = unstopForce * force.normalized * Time.fixedDeltaTime;
				Vector3 torque = unstopTorque * Vector3.Cross(force, Physics.gravity).normalized * Time.fixedDeltaTime;

				rb.AddForce(force);
				rb.AddTorque(torque);
			}
		}
		
	}
}
