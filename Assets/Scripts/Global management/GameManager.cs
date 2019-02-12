using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager { //a singleton
	

	private const int linesPerLevel = 5; //each description in the txt file is allowed 5 lines

	public const int numFields = 4; //achievements: (cubies,deaths,time,points)

	private Level currentLevel = null; //stores the level that was just loaded

	private Achievement[] requirements;

	private static GameManager instance = null;
	
	public static GameManager getInstance() {
		if(instance == null) {
			instance = new GameManager();
		}
		
		return instance;
	}
	
	private GameManager() {

		//exiting to the main menu always generates a quicksave
		Quicksave save = PlayerManager.getInstance().getQuicksave(SettingsManager.CurrentPlayer);
		if(save == null) {
			currentLevel = new Level(0,0); //pretend that the first level was just loaded
		} else {
			currentLevel = save.level;
		}

		changedScene(SceneManager.GetActiveScene(), LoadSceneMode.Single); //correct level parameters


		// load initial values
		SoundManager.getInstance().setMusic(currentLevel.stage);
		GameObject.FindWithTag("MainCamera").GetComponent<Script_Camera>().setLighting(currentLevel.stage);
		
		
		SceneManager.sceneLoaded += changedScene; //add changedScene() as a listener to new scenes being loaded

		parseRequirements();

		//Level.testLevel();
	}
	
	~GameManager() {
		SceneManager.sceneLoaded -= changedScene;
	}
	
	void changedScene(Scene scene, LoadSceneMode mode) {
		 //destroys the materials created in TextureManager; whenever a material is modified, a clone of it is made
		//already called when the scene changes
		Resources.UnloadUnusedAssets();
		
		Level newLevel = null;		
		
		//note that is SoundManager preserved across all scenes, while the camera script differs from scene to scene
		
		if(scene.name.Equals("Scene_Menu")) { //transitioned to the main menu
			
		} else { //transitioned to a level
			newLevel = getLevel(scene.name); 
			
			if(newLevel.stage != currentLevel.stage) { //change in stage
				//Debug.Log(currentLevel.stage + "->" + newLevel.stage);
				SoundManager.getInstance().setMusic(newLevel.stage);
			}
			
			currentLevel = newLevel;
		}
		
		Camera.main.GetComponent<Script_Camera>().setLighting(currentLevel.stage);
		TextureManager.getInstance().applyTextures(currentLevel.stage);

	}
	
	public Level getLevel() { //get current scene
		return currentLevel;
	}
	
	//saves the stage/substage corresponding to the scene name
	public Level getLevel(string sceneName) {
		string temp = sceneName.Split(new char[] {' '})[1]; //get second term of the scene name
        string[] temp2 = temp.Split(new char[] {'-'});
        
        int stage = Int32.Parse(temp2[0]); //stages and substages start indexing from 0
        int subStage = Int32.Parse(temp2[1]);
		
		return new Level(stage,subStage);
	}

	//return the requirements for getting achievements for a particular level
	public Achievement getRequirement(Level level) { //null return value means leveldata.txt is missing or invalid
		return requirements[level.Index];
	}

	//returns true if successful
	private bool parseRequirements() {
		string[] lines;
		string directory = Application.streamingAssetsPath + "/leveldata.txt";
		requirements = new Achievement[Level.numLevels];

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

			requirements[i] = parseRequirement(lines[i]);
			
			if(requirements[i] == null) { //null if parsing is unsuccessful

	        	requirements[i] = new Achievement(0,0,0,0);
				Debug.Log("invalid field at line " + i + " in leveldata.txt");
	        }
		}

		return true;
	}

	//parse a single achievement, received as plaintext; returns null if unsuccessful
	private Achievement parseRequirement(string line) {
		string[] fields = line.Split(new char[] {' '});
		
		if(fields.Length != numFields) {
			return null;
			
		} else {

			int cubies = 0;
			int deaths = 0;
			float time = 0;
			int points = 0;
			
			//leveldata.txt is in the format cubies/deaths/time/points
			bool success = Int32.TryParse(fields[0], out cubies) && //parse each field in the entry
						   Int32.TryParse(fields[1], out deaths) && 
						   Single.TryParse(fields[2], out time) &&
						   Int32.TryParse(fields[3], out points);

			if(success) {
				return new Achievement(cubies, deaths, time, points);
			} else {
				return null;
			}
		}
	}

}
