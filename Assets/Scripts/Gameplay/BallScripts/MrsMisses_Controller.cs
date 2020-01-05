using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrsMisses_Controller : Player_Controller
{
	private const int deathBonus = 15;

	public override int BonusPoints {
		get { return deathBonus * base.Deaths; }
	}

	public MrsMisses_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, 
							Player_Trails trailsScript, GameObject startPos, SphereCollider triggerCollider, 
							List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {
	}
}
