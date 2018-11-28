using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class Quicksave {

	public string player;
	public Level level;
	public SerializableVector3 position;
	public SerializableVector3 velocity;
	public SerializableVector3 angularVelocity;
	public SerializableVector3 startGravityDirection;
	public SerializableVector3 currentGravityDirection;
	//public SerializableVector3 cameraRotation;
	public List<bool> cubies; //true if the corresponding child in cubies still exists (i.e. has not been collected)
	public string startPos;
	public float time;
	public int deaths;

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
