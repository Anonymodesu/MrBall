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

		public string playerName;

		public Achievement[] records;

		public Quicksave quicksave;

		public int TotalCubies {
			get {
				return totalCubies;
			}
		}
		private int totalCubies;

		public PlayerProfile(string playerName) {
			this.playerName = playerName;
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
		findExistingPlayers();
	}

	public ICollection<string> getPlayers() {
		return playersProfiles.Keys;
	}

	
	//each player has their own file storing their best achievements for each level
	public void saveRecord(string playerName, Achievement current, Level level) {
		getProfile(playerName).saveRecord(current, level);
		saveProfiles();
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
		Debug.Log(getProfile(playerName).TotalCubies);
		return getProfile(playerName).TotalCubies;
	}

	public Quicksave getQuicksave(string playerName) {
		return getProfile(playerName).quicksave;
	}

	public void storeQuicksave(string playerName, Quicksave save) {
		getProfile(playerName).quicksave = save;
		saveProfiles();
	}

	public void deleteQuicksave(string playerName) {
		getProfile(playerName).quicksave = null;
		saveProfiles();
	}

	

	//saves list of profiles to file system
	private void saveProfiles() {

		//load all profiles into an array
		PlayerProfile[] profiles = new PlayerProfile[playersProfiles.Count];
		int i = 0;
		foreach(PlayerProfile profile in playersProfiles.Values) {
			profiles[i] = profile;
			i++;
		}

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(getPlayerDataPath());
		bf.Serialize(file, profiles);
		file.Close();
	}

	private PlayerProfile getProfile(string playerName) {

		//register new player
		if(!playersProfiles.ContainsKey(playerName)) {

			//existing profiles are loaded in findExistingPlayers()

			playersProfiles.Add(playerName, new PlayerProfile(playerName));
			saveProfiles();
			Debug.Log("creating new player data for " + playerName);
		} 

		return playersProfiles[playerName];
	}

	//load data of existing players into memory
	private void findExistingPlayers() {

		playersProfiles = new Dictionary<string, PlayerProfile>();

		//load existing player data if it exists
		if(File.Exists(getPlayerDataPath())) {

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(getPlayerDataPath(), FileMode.Open);
			PlayerProfile[] players = (PlayerProfile[]) bf.Deserialize(file);
			file.Close();

			foreach(PlayerProfile profile in players) {
				playersProfiles.Add(profile.playerName, profile);
			}
		}
	}

	//deletes the profile a player
	public void deletePlayer(string player) {
		playersProfiles.Remove(player);
		saveProfiles();
	}

	private static string getPlayerDataPath() {
		return Application.streamingAssetsPath + "/playerData.money";
	}

}
