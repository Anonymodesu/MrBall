using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrMister_Controller : Player_Controller
{
	private const float forceMultiplier = 2;
	private const float sigmoidConstant = 0.01f;
	private const float velocityThreshold = 0.01f;
	private const float soundThreshold = 0.3f;

	private GameObject mistEffect;
	private bool misting;

	public MrMister_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {
		Balls ballResources = GameObject.Find("Resources").GetComponent<Balls>();
		mistEffect = ballResources.MrMisterEffect;

		misting = false;
	}



    public override void Update() {
		base.Update();

		if(InputManager.getInput().buttonDown(Command.Special)) {
			misting = true;

		} else if (InputManager.getInput().buttonUp(Command.Special)) {
			misting = false;
		}

	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if(misting && rb.velocity.magnitude > velocityThreshold) {
			misting = false;
			float scale = (float) (sigmoidConstant / (sigmoidConstant + Math.Exp(-rb.velocity.magnitude)));
			Debug.Log(scale);
			GameObject mist = UnityEngine.Object.Instantiate(mistEffect, 
				playerScript.transform.position, 
				Quaternion.LookRotation(rb.velocity));

			mist.transform.localScale = scale * Vector3.one;

			rb.AddForce(forceMultiplier * -rb.velocity, ForceMode.VelocityChange);

			if(scale > soundThreshold) {
				SoundManager.getInstance().playSoundFX(SoundFX.MrMister, scale);
			}
		}
	}
}
