using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

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

		//save any new achievements
		public Achievement saveRecord(Achievement current, Achievement required, Level level) {
			Achievement oldBest = records[level.Index];

			//check whether an achievement record for the level already exists
			if(oldBest == null) {
				int numAchieved = current.numSatisfied(required);
				totalCubies += current.Cubies + numAchieved;
				records[level.Index] = current;

				Debug.Log("----");
				Debug.Log(current);
				Debug.Log(totalCubies);
			} else {
				//calculate best records among the old and new records for the level
				Achievement newBest = Achievement.Max(current, oldBest);

				//number of achievements acquired by the old best and new best 
				int numOldAchieved = oldBest.numSatisfied(required);
				int numNewAchieved = newBest.numSatisfied(required);

				totalCubies += (newBest.Cubies - oldBest.Cubies) + (numNewAchieved - numOldAchieved);

				records[level.Index] = newBest;				

				Debug.Log("----");
				Debug.Log(oldBest);
				Debug.Log(current);
				Debug.Log(newBest);
				Debug.Log(totalCubies);
			}

			return records[level.Index];
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

	//returns the new set of records held by the player
	public Achievement saveRecord(string playerName, Achievement current, Achievement required, Level level) {
		Achievement newRecord = getProfile(playerName).saveRecord(current, required, level);
		saveProfiles();
		return newRecord;
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
			FileStream file = null;

			try{
				file = File.Open(getPlayerDataPath(), FileMode.Open);
				PlayerProfile[] players = (PlayerProfile[]) bf.Deserialize(file);

				foreach(PlayerProfile profile in players) {
					playersProfiles.Add(profile.playerName, profile);
				}

			} catch (SerializationException e) {
				Debug.Log(e.Message);

			} finally {
				if(file != null) {
					file.Close();
				}
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
