using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrBowl_Move : Player_Move {

	protected override float brakeStrength {
		get { return base.brakeStrength * 1.5f; }
	}

	public MrBowl_Move(Script_Player_Loader playerScript) : base(playerScript) {

	}
}
