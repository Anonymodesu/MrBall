using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerManager {

	[Serializable]
	private class PlayerProfile {

		public Achievement[] records;

		public Quicksave quicksave;

		public int TotalCubies {
			get {
				return totalCubies;
			}
		}
		private int totalCubies;

		public PlayerProfile() {
			records = new Achievement[Level.numLevels];
			quicksave = null;
			totalCubies = 0;
		}

		public void saveRecord(Achievement newRecord, Level level) {

			//check whether an achievement record for the level already exists
			if(records[level.Index] == null) {
				totalCubies += newRecord.Cubies;
				records[level.Index] = newRecord;

			} else {
				totalCubies += newRecord.Cubies - records[level.Index].Cubies;
				records[level.Index] = Achievement.Max(records[level.Index], newRecord);
			}
		}
	}


	private Dictionary<string, PlayerProfile> playersProfiles;

	private static PlayerManager instance = null;

	public static PlayerManager getInstance() {
		if(instance == null) {
			instance = new PlayerManager();
		}
		
		return instance;
	}

	private PlayerManager() {
		playersProfiles = new Dictionary<string, PlayerProfile>();

		//load data of existing players into memory
		foreach(string playerPath in Directory.GetFiles(Application.streamingAssetsPath, "*.dat")) {
			string player = Path.GetFileNameWithoutExtension(playerPath);
			getProfile(player);
		}
	}

	public ICollection<string> getPlayers() {
		return playersProfiles.Keys;
	}

	
	//each player has their own file storing their best achievements for each level
	public void saveRecord(string playerName, Achievement current, Level level) {
		getProfile(playerName).saveRecord(current, level);
		saveProfile(playerName);
	}
	
	//retrieves all the records for a player
	public Achievement[] getRecords(string playerName) {
		return getProfile(playerName).records;
	}

	//retrieves an achievement for a level for a player
	public Achievement getRecord(string playerName, Level level) {
		return getRecords(playerName)[level.Index];
	}


	public int getTotalCubies(string playerName) {
		return getProfile(playerName).TotalCubies;
	}

	public Quicksave getQuicksave(string playerName) {
		return getProfile(playerName).quicksave;
	}

	public void storeQuicksave(string playerName, Quicksave save) {
		getProfile(playerName).quicksave = save;
		saveProfile(playerName);
	}

	public void deleteQuicksave(string playerName) {
		getProfile(playerName).quicksave = null;
		saveProfile(playerName);
	}

	

	//save player profile to file system
	private void saveProfile(string playerName) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(getProfilePathName(playerName));
		bf.Serialize(file, getProfile(playerName));
		file.Close();
	}

	private PlayerProfile getProfile(string playerName) {

		//if the player data has not been read for that player yet
		if(!playersProfiles.ContainsKey(playerName)) {
			
			//player has played some levels before
			if(File.Exists(getProfilePathName(playerName))) {

				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(getProfilePathName(playerName), FileMode.Open);
				playersProfiles.Add(playerName, (PlayerProfile) bf.Deserialize(file));
				file.Close();

			//new player
			} else {
				playersProfiles.Add(playerName, new PlayerProfile());
				Debug.Log("creating new player data file for " + playerName);
			}
		} 

		return playersProfiles[playerName];
	}

	//deletes the profile a player
	public void deletePlayer(string player) {
		string path = getProfilePathName(player);

		if(File.Exists(path)) {
			File.Delete(path);
		}

		if(File.Exists(path + ".meta")) {
			File.Delete(path + ".meta");	
		}

		playersProfiles.Remove(player);
	}

	private static string getProfilePathName(string playerName) {
		return Application.streamingAssetsPath + "/" + playerName + ".dat";
	}

}
