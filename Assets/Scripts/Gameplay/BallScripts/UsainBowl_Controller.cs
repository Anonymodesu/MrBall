using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsainBowl_Controller : Player_Controller {
	public UsainBowl_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, Player_Trails trailsScript, 
							GameObject startPos, SphereCollider triggerCollider, List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

	}

	protected override void die() {
		base.die();
		((UsainBowl_Move) movementScript).superSaiyanable = true;
	}

	public override Quicksave storeQuickSave() {
		Quicksave_UsainBowl save = new Quicksave_UsainBowl(base.storeQuickSave(), ((UsainBowl_Move) movementScript).superSaiyanable);
		PlayerManager.getInstance().storeQuicksave(player, save);
		return save;
	}

	protected override Quicksave loadQuickSave() {
		Debug.Log("subclass quicksave loaded");
		Quicksave_UsainBowl save = (Quicksave_UsainBowl) base.loadQuickSave();
		((UsainBowl_Move) movementScript).superSaiyanable = save.superSaiyanable;
		return save;
	}
}
