using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroBall_Move : Player_Move {

	public GyroBall_Move(Script_Player_Loader playerScript) : base(playerScript) {

	}

	//for purposes of movement, Gyro Ball is always considered to be on ground
    protected override bool contactingRamp(List<GameObject> contacts) {
    	return true;
    }
}
