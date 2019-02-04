using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//generates speed lines when the player is moving faster than a threshold velocity
public interface Player_Trails {
	void processTrails();
	void updateColours(string tag);
}
