using System.Collections;
using System.Collections.Generic;

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
}
