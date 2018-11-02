using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level {

	public int stage;
	public int substage;
		
	public Level(int stage, int substage) {
		this.stage = stage;
		this.substage = substage;
	}
		
	public override string ToString() {
		return "Scene_Stage " + stage + "-" + substage;
	}
}
