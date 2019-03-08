using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NielsBall_Controller : Player_Controller {

	private bool reversible;

	public NielsBall_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		} else {
			reversible = true;
		}
	}
	
	public override void Update() {
		base.Update();

		if(reversible && Time.timeScale != 0 && InputManager.getInput().buttonDown(Command.Special)) {
			reverseGravity();
		}
	}

	protected override void die() {
		base.die();
		reversible = true;

	}

	private void reverseGravity() {
		reversible = false;
		processGravity(-Physics.gravity.normalized, Physics.gravity.normalized);
	}

	public override Quicksave storeQuickSave() {
		Quicksave_NielsBall save = new Quicksave_NielsBall(base.storeQuickSave(), reversible);
		PlayerManager.getInstance().storeQuicksave(player, save);
		return save;
	}

	private void loadQuickSave() {
		Quicksave_NielsBall save = (Quicksave_NielsBall) PlayerManager.getInstance().getQuicksave(player);
		reversible = save.reversible;
	}
}
