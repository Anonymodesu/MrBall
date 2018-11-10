using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Achievement { //used for both storing requirements for a stage's achievements as well as player records

	public int cubies;
	public int deaths;
	public int time;
	public int points;
	
	public Achievement(int cubies, int deaths, int time, int points) {
		this.cubies = cubies;
		this.deaths = deaths;
		this.time = time;
		this.points = points;
	}

	//returns the best records between the two achievements
	public static Achievement Max(Achievement a, Achievement b) {
		Achievement newAchievement = new Achievement(0,0,0,0);

		newAchievement.cubies = Math.Max(a.cubies, b.cubies);
		newAchievement.deaths = Math.Min(a.deaths, b.deaths);	
		newAchievement.time = Math.Min(a.time, b.time);
		newAchievement.points = Math.Max(a.points, b.points);

		return newAchievement;
	}
}
