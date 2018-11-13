using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Achievement { //used for both storing requirements for a stage's achievements as well as player records

	private int cubies;
	private int deaths;
	private float time;
	private int points;

	public int Cubies {
		get { return cubies; }
	}

	public int Deaths {
		get { return deaths; }
	}

    public string TimeString {
        get { return time.ToString("0.00"); }
    }

    public float Time {
        get { return time; }
    }

    public int Points {
		get { return points; }
	}
	
	public Achievement(int cubies, int deaths, float time, int points) {
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
