using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NielsBall_Controller : Player_Controller {

	private const float cooldown = 0.6f;
	private bool reversible;


	public NielsBall_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps, SphereCollider physicCollider)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		reversible = true;
	}
	
	public override void Update() {
		base.Update();

		if(reversible && Time.timeScale != 0 && InputManager.getInput().buttonDown(Command.Special)) {
			playerScript.StartCoroutine(reverseGravity());
		}
	}

	private IEnumerator reverseGravity() {
		reversible = false;
		processGravity(-Physics.gravity.normalized, Physics.gravity.normalized);
		yield return new WaitForSeconds(cooldown);
		reversible = true;
	}
}
