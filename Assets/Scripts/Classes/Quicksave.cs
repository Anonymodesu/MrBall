using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Quicksave {

	public readonly string player;
	public readonly Level level;
	public readonly SerializableVector3 position;
	public readonly SerializableVector3 velocity;
	public readonly SerializableVector3 angularVelocity;
	public readonly SerializableVector3 startGravityDirection;
	public readonly SerializableVector3 currentGravityDirection;
	//public SerializableVector3 cameraRotation;
	public readonly List<bool> cubies; //true if the corresponding child in cubies still exists (i.e. has not been collected)
	public readonly string startPos;
	public readonly float time;
	public readonly int deaths;

	public Quicksave(string player, Level level, Vector3 position, Vector3 velocity, Vector3 angularVelocity,
					Vector3 startGravityDirection, Vector3 currentGravityDirection, List<bool> cubies,
					string startPos, float time, int deaths) {
		this.player = player;
		this.level = level;
		this.position = position;
		this.velocity = velocity;
		this.angularVelocity = angularVelocity;
		this.startGravityDirection = startGravityDirection;
		this.currentGravityDirection = currentGravityDirection;
		this.cubies = cubies;
		this.startPos = startPos;
		this.time = time;
		this.deaths = deaths;
	}
}
