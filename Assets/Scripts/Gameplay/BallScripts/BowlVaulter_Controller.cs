using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlVaulter_Controller : Player_Controller {
	private const float vaultStrength = 12;
	private const float vaultThreshold = 0.5f;

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
			playerScript.StartCoroutine(vault());
		}
	}

	private IEnumerator vault() {
		vaultable = false;
		jumpScript.cancelVelocity(-Physics.gravity);
		rb.AddForce(vaultStrength * (-Physics.gravity.normalized), ForceMode.Impulse);
		//rb.AddTorque(0.4f * vaultStrength * rb.velocity.normalized, ForceMode.Impulse);
		updateTrails("Perpendicular");
		SoundManager.getInstance().playSoundFX(SoundFX.BowlVaulter);

		GameObject effect = UnityEngine.Object.Instantiate(explosionEffect);
		while(effect != null) {
			effect.transform.position = playerScript.transform.position;
			effect.transform.rotation = Quaternion.LookRotation(rb.velocity);
			yield return null;
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
