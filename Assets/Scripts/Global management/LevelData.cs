using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData  {

	private static LevelData instance = null;

	private Dictionary<Level, string> levelDescriptions;
	private Dictionary<Level, string> levelNames;

	private LevelData() {
		levelDescriptions = new Dictionary<Level, string>();
		levelNames = new Dictionary<Level, string>();

		levelDescriptions.Add(new Level(0,0), 
@"Use W,A,S,D to move.
Use the mouse to control the camera.
Navigate to the red panel to claim victory.");
		levelDescriptions.Add(new Level(0,1), 
@"Left-click to jump. Try not to fall off!");
		levelDescriptions.Add(new Level(0,2), 
@"It is harder to control your speed on sloped ramps");
		levelDescriptions.Add(new Level(0,3), 
@"Yellow ramps make you jump a lot higher. 
Try vaulting over these walls.");
		levelDescriptions.Add(new Level(0,4), 
@"Mr.Ball only needs to be in contact with a yellow ramp to use its effect.");
		levelDescriptions.Add(new Level(0,5), 
@"White ramps are checkpoints. They save your progress in the level.
Hurray!");
		levelDescriptions.Add(new Level(0,6), 
@"It can be difficult to come to a full stop on narrow ramps.
You can right-click to brake.");
		levelDescriptions.Add(new Level(0,7), 
@"The floating cubes, also known as cubies, grant a hefty amount of points if you collect them, among other things...");
		levelDescriptions.Add(new Level(0,8), 
@"Checkpoints are your friends.
Most of the time, anyway.");
		levelDescriptions.Add(new Level(0,9), 
@"The final level of each stage ramps up the difficulty.
Feel free to skip these if you have too much trouble.");


		levelDescriptions.Add(new Level(1,0), 
@"Purple ramps induce extreme speed.
Use with caution.");
		levelDescriptions.Add(new Level(1,1), 
@"Rest assured, each cubie has been proven collectable by calm and collected cubie collectors.");
		levelDescriptions.Add(new Level(1,2), 
@"There may be multiple paths you can take to finish a level. 
Some are faster (and less obvious) than others.");
		levelDescriptions.Add(new Level(1,3), 
@"Unless you're a very brave person, you should attempt these levels in order.");
		levelDescriptions.Add(new Level(1,4), 
@"The crosshair can be used to navigate to hard-to-reach areas.");
		levelDescriptions.Add(new Level(1,5), 
@"If Mr. Ball is in contact with many surfaces, he will take advantage of any special effects that they have to offer.");
		levelDescriptions.Add(new Level(1,6), 
@"The Sun just unleashed a violent burst of radiation!	
The once docile ramps have begun moving of their own will!");
		levelDescriptions.Add(new Level(1,7), 
@"Although other balls provide nice bonuses, good ol' Mr. Ball is sufficient to complete every level in the game.");
		levelDescriptions.Add(new Level(1,8), 
@"Having sufficient angular momentum is required to scale certain obstacles.
Which means Mr. Ball should have a run-up before attempting the jumps in this level.");
		levelDescriptions.Add(new Level(1,9), 
@"Mr. Ball was inspired by a very old game called 'Marble Blast Gold'.");

		levelDescriptions.Add(new Level(2,0), 
@"Antimatter rings jolt Mr. Ball in the direction that they are facing.");
		levelDescriptions.Add(new Level(2,1), 
@"Although Mr. Ball can't change directions mid-air, he has limited manoeuvrability against walls.");
		levelDescriptions.Add(new Level(2,2), 
@"Antimatter rings give Mr. Ball a push, in addition to his current velocity.");
		levelDescriptions.Add(new Level(2,5), 
@"Brace yourself.");

		levelDescriptions.Add(new Level(3,0), 
@"Orange ramps propel Mr. Ball in the direction that they are facing when he jumps on them.");
		levelDescriptions.Add(new Level(3,1), 
@"If Mr.Ball is touching both yellow and orange ramps, both types of jump effects will be applied.");
		levelDescriptions.Add(new Level(3,2), 
@"The Sun just unleashed another a burst of radiation!	
Some ramps have mutated, but functionally behave the same.");
		levelDescriptions.Add(new Level(3,3), 
@"Some people ask me how Mr. Ball rolls with such elegance. 
I say that it's with state-of-the-art gyroscope technology.");
		levelDescriptions.Add(new Level(3,4), 
@"In case you were wondering, cubies also gyrate using state-of-the-art gyroscope tech.");

		levelDescriptions.Add(new Level(4,0), 
@"Blue ramps change gravity by considering the normal to surface from a raycast originating from the centre of...
Actually, it's best if you try it out for yourself.");
		levelDescriptions.Add(new Level(4,1), 
@"Using the same blue ramp may lead to very different gravities, in the same way that the same orange ramp may push you in different directions.");
		levelDescriptions.Add(new Level(4,2), 
@"The dude who implemented these gravity ramps spent a lot of time on them.
He hopes that you enjoy playing around with them.");
		levelDescriptions.Add(new Level(4,3), 
@"You can usually use the number of checkpoints in a level to estimate its difficulty.
Usually.");
		levelDescriptions.Add(new Level(4,4), 
@"There are several ways to approach this level.");

		levelNames.Add(new Level(0,0), "Rolling");
		levelNames.Add(new Level(0,1), "Jumping");
		levelNames.Add(new Level(0,2), "Rolling hills");
		levelNames.Add(new Level(0,3), "Super jumping");
		levelNames.Add(new Level(0,4), "Super jumping 2");
		levelNames.Add(new Level(0,5), "Checkpoints");
		levelNames.Add(new Level(0,6), "Braking");
		levelNames.Add(new Level(0,7), "Need for speed");
		levelNames.Add(new Level(0,8), "Betrayal");
		levelNames.Add(new Level(0,9), "Difficulty spike");

		levelNames.Add(new Level(1,0), "Rolling thunder");
		levelNames.Add(new Level(1,1), "Baby steps");
		levelNames.Add(new Level(1,2), "Shortcuts");
		levelNames.Add(new Level(1,3), "The hole");
		levelNames.Add(new Level(1,4), "Take aim");
		levelNames.Add(new Level(1,5), "Super jumping");
		levelNames.Add(new Level(1,6), "Teenage Mutant Ninja Ramps");
		levelNames.Add(new Level(1,7), "Super jumping 2");
		levelNames.Add(new Level(1,8), "Rollercoaster");
		levelNames.Add(new Level(1,9), "Mr. Cube");

		levelNames.Add(new Level(2,0), "Falling, with style");
		levelNames.Add(new Level(2,1), "Parkour");
		levelNames.Add(new Level(2,2), "Slice serve");
		levelNames.Add(new Level(2,5), "Two-way street");
		levelNames.Add(new Level(2,7), "Adventure time");
		levelNames.Add(new Level(2,8), "Every 10 seconds");


		levelNames.Add(new Level(3,0), "Perpendicular jumping");
		levelNames.Add(new Level(3,1), "Ultra jumping");
		levelNames.Add(new Level(3,2), "Mr. Balls");
		levelNames.Add(new Level(3,3), "Mr. Cube Senior");
		levelNames.Add(new Level(3,4), "So close, yet so far");

		levelNames.Add(new Level(4,0), "Change of perspective");
		levelNames.Add(new Level(4,1), "Walking straight");
		levelNames.Add(new Level(4,2), "Around we go");
		levelNames.Add(new Level(4,3), "Mr. Cube Junior");
		levelNames.Add(new Level(4,4), "Ever get that feeling of");
	}

	public static LevelData getInstance() {
		if(instance == null) {
			instance = new LevelData();
		}

		return instance;
	}


	public string getLevelName(Level level) {
		if(levelNames.ContainsKey(level)) {
			return levelNames[level];
		} else {
			return "missing name for " + level.ToString();
		}
	}

	public string getLevelDescription(Level level) {
		if(levelDescriptions.ContainsKey(level)) {
			return levelDescriptions[level];
		} else {
			return "missing description for " + level.ToString();
		}
	}
}
