using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;


public class AchievementManager {

	public const int numFields = 4; //achievements: (cubies,deaths,time,points)
	public const int numSubstages = 5;
    public const int numStages = 6;
	public const int numLevels = numStages * numSubstages;

	private static AchievementManager instance = null;

	public static AchievementManager getInstance() {
		if(instance == null) {
			instance = new AchievementManager();
		}
		
		return instance;
	}
	
	//return the requirements for getting achievements for a particular level
	public Achievement getRequirement(Level level) { //null return value means leveldata.txt is missing or invalid
		string[] lines;
		string directory = Application.streamingAssetsPath + "/leveldata.txt";
		
		if(File.Exists(directory)) {
			lines =  System.IO.File.ReadAllLines(directory);

		} else {
			return null;
		}

		int index = numSubstages * level.stage + level.substage; //stage starts indexing from 0; substage starts indexing from 1
		
		if(index >= numLevels) {
			Debug.Log("incorrect number of lines in leveldata.txt");
			return null;
		}
				
		//achievements.txt is in the format cubies/time/points
		Achievement achievement = new Achievement(0,0,0,0);
		
		if(parseAchievement(lines[index], achievement)) { //parse achievement and save value in achievement
			   
			return achievement;
               
        } else { //invalid field(s)
			Debug.Log("invalid field at line " + index + " in leveldata.txt");
            return null;
        }

	}
	
	//each player has their own file storing their best achievements for each level
	public void saveAchievement(string playerName, Achievement current, Level level) {
		string[] lines;
		string directory = getPathName(player);
		
		if(File.Exists(directory)) {
			lines = System.IO.File.ReadAllLines(directory);

		} else { //no player data yet
			Debug.Log(playerName + " does not exist. creating new player data file");
			resetAchievements(playerName);
			lines =  System.IO.File.ReadAllLines(directory);
		}
		
		if(lines.Length != numLevels) {
			Debug.Log("incorrect number of lines in data file for " + playerName + ". resetting file");
			resetAchievements(playerName);
			lines =  System.IO.File.ReadAllLines(directory);
		}
		
		int index = level.stage * numSubstages + level.substage; 
		
		//write to player's data file; each line in the format: cubies deaths time points
		if(lines[index] != "") { //player has played this level already

			Achievement record = new Achievement(0,0,0,0);
				
			if(parseAchievement(lines[index], record)) { //save recorded achievement into record
					   
			   //update new records if applicable
				record.cubies = Math.Max(record.cubies, current.cubies);
				record.deaths = Math.Min(record.deaths, current.deaths);	
				record.time = Math.Min(record.time, current.time);
				record.points = Math.Max(record.points, current.points);
					
				lines[index] = record.cubies + " " + record.deaths + " " + record.time + " " + record.points; 
				System.IO.File.WriteAllLines(directory, lines); //write updated achievements to txt file
					
			} else {
				Debug.Log("incorrect format of fields at " + index + ". resetting player data for " + playerName);
				resetAchievements(playerName);
			}
			
		} else { //current entry is empty; player has not played this level
			lines[index] = current.cubies + " " + current.deaths + " " + current.time + " " + current.points; 
			System.IO.File.WriteAllLines(directory, lines); //write updated achievements to txt file
		}
		

	}
	
	private void resetAchievements(string playerName) {
		StringBuilder sb = new StringBuilder();
		for(int i = 0; i < numStages; i++) {
			
			for(int j = 0; j < numSubstages; j++) {
				sb.Append("\n");
			}
			
		}
		
		System.IO.File.WriteAllText(Application.streamingAssetsPath + "/" + playerName + "_data.txt", sb.ToString());
	}
	
	public Achievement[] getAchievements(string playerName) {
		string[] lines = null;
		string directory = getPathName(player);
		
		if(File.Exists(directory)) {
			lines =  System.IO.File.ReadAllLines(directory);

		} else {
			Debug.Log(playerName + "does not exist yet. resetting player achievements");
			resetAchievements(playerName);
			lines = System.IO.File.ReadAllLines(directory);
		}
		
		if(lines.Length != numLevels) {
			Debug.Log("incorrect number of lines in data file for " + playerName + ". resetting file");
			resetAchievements(playerName);
			lines = System.IO.File.ReadAllLines(directory);
		}
		
		Achievement[] achievements = new Achievement[numLevels];
		for(int i = 0; i < numLevels; i++) {
			
			Achievement current = new Achievement(0,0,0,0);
			
			if(lines[i] == "") { //empty record
				achievements[i] = null;
				
			} else if(parseAchievement(lines[i], current)) { //saved record
				achievements[i] = current;
				
			} else { //invalid record
				Debug.Log("invalid achievement at line " + i + " for player " + playerName);
				achievements[i] = null;
			}
		}
		
		return achievements;
	}
	
	//parse a single achievement, received as plaintext; saves parsed values in achievement; returns true iff successful
	private bool parseAchievement(string line, Achievement achievement) {
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

	private static string getPathName(string player) {
		return Application.streamingAssetsPath + "/" + playerName + ".dat"
	}
}
