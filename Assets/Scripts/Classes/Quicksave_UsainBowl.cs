using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Quicksave_UsainBowl : Quicksave {

	public readonly bool superSaiyanable;

	public Quicksave_UsainBowl(Quicksave baseSave, bool superSaiyanable) : base(baseSave.level, baseSave.position, baseSave.velocity, baseSave.angularVelocity,
					baseSave.startGravityDirection, baseSave.currentGravityDirection, baseSave.cubies,
					baseSave.startPos, baseSave.time, baseSave.deaths, baseSave.rampAnimationTimes, baseSave.ballUsed) {

		this.superSaiyanable = superSaiyanable;
	}
}
