using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Quicksave_NielsBall : Quicksave {

	public readonly bool reversible;

	public Quicksave_NielsBall(Quicksave baseSave, bool reversible) : base(baseSave.level, baseSave.position, baseSave.velocity, baseSave.angularVelocity,
					baseSave.startGravityDirection, baseSave.currentGravityDirection, baseSave.cubies,
					baseSave.startPosIDs, baseSave.time, baseSave.deaths, baseSave.rampAnimationTimes, baseSave.ballUsed) {

		this.reversible = reversible;
	}
}
