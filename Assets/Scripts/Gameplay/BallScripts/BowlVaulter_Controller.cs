using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlVaulter_Controller : Player_Controller {
	private const float vaultStrength = 12;

	private GameObject explosionEffect;

	private bool vaultable;

	public BowlVaulter_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, Player_Trails trailsScript, 
							GameObject startPos, SphereCollider triggerCollider, List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		} else {
			vaultable = true;
		}

		explosionEffect = GameObject.Find("Resources").GetComponent<Balls>().BowlVaulterEffect;

	}

	public override void Update() {
		base.Update();

		if(Time.timeScale != 0 && vaultable && InputManager.getInput().buttonDown(Command.Special)) {
			vault();
		}
	}

	private void vault() {
		if(rb.velocity != Vector3.zero) {
			vaultable = false;
			rb.AddForce(vaultStrength * rb.velocity.normalized, ForceMode.Impulse);
			rb.AddTorque(0.4f * vaultStrength * rb.velocity.normalized, ForceMode.Impulse);
			updateTrails("Perpendicular");
			UnityEngine.Object.Instantiate(explosionEffect, playerScript.transform.position, Quaternion.LookRotation(rb.velocity));
		}
	}

	protected override void die() {
		base.die();
		vaultable = true;
	}

	public override Quicksave storeQuickSave() {
		Quicksave_BowlVaulter save = new Quicksave_BowlVaulter(base.storeQuickSave(), vaultable);
		PlayerManager.getInstance().storeQuicksave(player, save);
		return save;
	}

	private void loadQuickSave() {
		Quicksave_BowlVaulter save = (Quicksave_BowlVaulter) PlayerManager.getInstance().getQuicksave(player);
		vaultable = save.vaultable;
	}
}
