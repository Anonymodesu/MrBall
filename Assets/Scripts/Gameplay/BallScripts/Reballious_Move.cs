using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reballious_Move : Player_Move {

	// Use this for initialization
	public Reballious_Move(Script_Player_Loader playerScript) : base(playerScript) {

	}

	public override void processNextInstruction() {
		base.processNextInstruction();

		if(lastInstruction.forward > 0) {
			lastInstruction.forward = 0;
		}

		if(lastInstruction.right > 0) {
			lastInstruction.right = 0;
		}
	}
}
