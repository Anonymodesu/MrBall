using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intangiball_Controller : Player_Controller {

	private const float vanishThreshold = 0.4f;

	private SphereCollider physicCollider;
	private bool vanishing;
	private Intangiball_Helper helper;
	private GameObject poofEffect;
	private Renderer renderer;
	private Material opaqueMaterial;
	private Material transparentMaterial;

	public Intangiball_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps, SphereCollider physicCollider)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		this.physicCollider = physicCollider;
		vanishing = false;

		Balls ballResources = GameObject.Find("Resources").GetComponent<Balls>();
		renderer = playerScript.GetComponent<Renderer>();
		helper = UnityEngine.Object.Instantiate(ballResources.IntangiballHelper, playerScript.transform, false)
												.GetComponent<Intangiball_Helper>();
		poofEffect = ballResources.IntangiballPoof;

		opaqueMaterial = renderer.material;
		transparentMaterial = ballResources.IntangiballTransparent;
		
	}

	public override void Update() {
		base.Update();

		if(Time.timeScale != 0) {

			//helper.transform.position = playerScript.transform.position;

			if(InputManager.getInput().buttonDown(Command.Special)) {

				if(!vanishing) { //onl
					UnityEngine.Object.Instantiate(poofEffect, playerScript.transform.position, Quaternion.identity);
				}

				vanishing = true;
				renderer.material = transparentMaterial;



			} else if(vanishing && helper.Colliding) {
				//do nothing
				//can't unvanish if currently passing through objects

			} else if (vanishing) { //special button has been released and not colliding with anything
				vanishing = false;
				renderer.material = opaqueMaterial;
				
			}
		}
	}

	public override void FixedUpdate() {
		base.FixedUpdate();

		if(vanishing) { //this stuff is put here instead of Update() so physics updates in time
			physicCollider.enabled = false;
			triggerCollider.enabled = false;
			contacts.Clear();


		} else {
			physicCollider.enabled = true;
			triggerCollider.enabled = true;
		}

		//helper.transform.position = playerScript.transform.position;
	}


}
