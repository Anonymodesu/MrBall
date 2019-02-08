using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Quicksave_BowlVaulter : Quicksave {

	public readonly bool vaultable;

	public Quicksave_BowlVaulter(Quicksave baseSave, bool vaultable) : base(baseSave.level, baseSave.position, baseSave.velocity, baseSave.angularVelocity,
					baseSave.startGravityDirection, baseSave.currentGravityDirection, baseSave.cubies,
					baseSave.startPosIDs, baseSave.time, baseSave.deaths, baseSave.rampAnimationTimes, baseSave.ballUsed) {

		this.vaultable = vaultable;
	}
}
