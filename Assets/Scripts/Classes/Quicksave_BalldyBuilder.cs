using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Quicksave_BalldyBuilder : Quicksave {

	public readonly List<int> checkpointParents;

	public Quicksave_BalldyBuilder(Quicksave baseSave, List<int> checkpointParents) 
					: base(baseSave.level, baseSave.position, baseSave.velocity, baseSave.angularVelocity,
					baseSave.startGravityDirection, baseSave.currentGravityDirection, baseSave.cubies,
					baseSave.startPosIDs, baseSave.time, baseSave.deaths, baseSave.rampAnimationTimes, baseSave.ballUsed) {

		this.checkpointParents = checkpointParents;
	}
}
