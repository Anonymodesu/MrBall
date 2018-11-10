using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class AchievementManager {

	public const int numFields = 4; //achievements: (cubies,deaths,time,points)
	public const int numSubstages = 5;
    public const int numStages = 5;
	public const int numLevels = numStages * numSubstages;

	private Achievement[] requirements;
	private Dictionary<string, Achievement[]> playerAchievements;

	private static AchievementManager instance = null;

	public static AchievementManager getInstance() {
		if(instance == null) {
			instance = new AchievementManager();
		}
		
		return instance;
	}

	private AchievementManager() {
		requirements = null;
		playerAchievements = new Dictionary<string, Achievement[]>();
	}
	
	//return the requirements for getting achievements for a particular level
	public Achievement getRequirement(Level level) { //null return value means leveldata.txt is missing or invalid
		if(requirements == null) {
			parseRequirements();
		}
					
		return requirements[numSubstages * level.stage + level.substage];
	}
	
	//each player has their own file storing their best achievements for each level
	public void saveAchievement(string playerName, Achievement current, Level level) {

		Achievement[] achievements = getAchievements(playerName);
		int index = level.stage * numSubstages + level.substage; 

		//player has not completed this level yet
		if(achievements[index] == null) {
			achievements[index] = current;

		} else { //retrieve the best records among the two
			achievements[index] = Achievement.Max(achievements[index], current);
		}

		//save to file system
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(getPathName(playerName));
		bf.Serialize(file, achievements);
		file.Close();

	}
	
	public Achievement[] getAchievements(string playerName) {

		//if the player data has not been read for that player yet
		if(!playerAchievements.ContainsKey(playerName)) {
			
			//player has played some levels before
			if(File.Exists(getPathName(playerName))) {

				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(getPathName(playerName), FileMode.Open);
				playerAchievements.Add(playerName, (Achievement[]) bf.Deserialize(file));
				file.Close();

			//new player
			} else {
				playerAchievements.Add(playerName, new Achievement[numLevels]);
				Debug.Log("creating new player data file for " + playerName);
			}

		} 

		return playerAchievements[playerName];
	}

	//returns true if successful
	private bool parseRequirements() {
		string[] lines;
		string directory = Application.streamingAssetsPath + "/leveldata.txt";
		requirements = new Achievement[numLevels];

		if(File.Exists(directory)) {
			lines =  System.IO.File.ReadAllLines(directory);
		} else {
			Debug.Log("missing leveldata.txt");
			return false;
		}

		if(lines.Length < requirements.Length) {
			Debug.Log("incorrect number of lines in leveldata.txt");
			return false;
		}

		for(int i = 0; i < requirements.Length; i++) {
			//leveldata.txt is in the format cubies/deaths/time/points
			requirements[i] = new Achievement(0,0,0,0);
			
			if(parseRequirement(lines[i], requirements[i])) { //returns true if parsing is successful
				   		               
	        } else { //invalid field(s)
				Debug.Log("invalid field at line " + i + " in leveldata.txt");
	        }
		}

		return true;
	}


	//parse a single achievement, received as plaintext; saves parsed values in achievement; returns true iff successful
	private bool parseRequirement(string line, Achievement achievement) {
		string[] fields = line.Split(new char[] {' '});
		
		if(fields.Length != numFields) {
			return false;
			
		} else {
			
			return Int32.TryParse(fields[0], out achievement.cubies) && //parse each field in the entry
				   Int32.TryParse(fields[1], out achievement.deaths) && 
				   Int32.TryParse(fields[2], out achievement.time) &&
				   Int32.TryParse(fields[3], out achievement.points);
		}
	}


	private static string getPathName(string playerName) {
		return Application.streamingAssetsPath + "/" + playerName + ".dat";
	}
}
