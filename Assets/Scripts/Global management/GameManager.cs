using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager { //a singleton
	
		
	private const int linesPerLevel = 5; //each description in the txt file is allowed 5 lines
	
	private Level currentLevel = null; //stores the level that was just loaded
	private Dictionary<Level, string> levelDescriptions;
	private Dictionary<Level, string> levelNames;
	
	private static GameManager instance = null;
	
	public static GameManager getInstance() {
		if(instance == null) {
			instance = new GameManager();
		}
		
		return instance;
	}
	
	private GameManager() {		
		currentLevel = new Level(0,0); //pretend that the first level was just loaded
		
		// load initial values
		SoundManager.getInstance().setMusic(currentLevel.stage);
		GameObject.FindWithTag("MainCamera").GetComponent<Script_Camera>().setLighting(currentLevel.stage);
		
		changedScene(SceneManager.GetActiveScene(), LoadSceneMode.Single); //modify to correct level parameters
		
		SceneManager.sceneLoaded += changedScene; //add changedScene() as a listener to new scenes being loaded

		parseLevelData();
		//Level.testLevel();
	}
	
	~GameManager() {
		SceneManager.sceneLoaded -= changedScene;
	}
	
	void changedScene(Scene scene, LoadSceneMode mode) {
		 //destroys the materials created in TextureManager; whenever a material is modified, a clone of it is made
		//already called when the scene changes
		//Resources.UnloadUnusedAssets();
		
		Level newLevel = null;		
		Script_Camera currentCamera = GameObject.FindWithTag("MainCamera").GetComponent<Script_Camera>(); //polymorphism :0
		
		//note that SoundManager preserved across all scenes, while the camera script differs from scene to scene
		
		if(scene.name.Equals("Scene_Menu")) { //transitioned to the main menu
			//do nothing
			
		} else { //transitioned to a level
			newLevel = getLevel(scene.name); 
			
			if(newLevel.stage != currentLevel.stage) { //change in stage
				//Debug.Log(currentLevel.stage + "->" + newLevel.stage);
				SoundManager.getInstance().setMusic(newLevel.stage);
			}
			
			currentLevel = newLevel;
		}
		
		currentCamera.setLighting(currentLevel.stage);
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
	
	//returns the flavour text displayed at the beginning of each level
	public string getLevelDescription(Level level) {
		if(levelDescriptions == null) {
			return "missing description for " + level.ToString();
		} else {
			return levelDescriptions[level];
		}
	}

	public string getLevelName(Level level) {
		if(levelNames == null) {
			return "missing level name for " + level.ToString();
		} else {
			return levelNames[level];
		}
	}

	private void parseLevelData() {
		string[] levelDescriptions = null;
		string[] levelNames = null;

		//housekeeping
		try {
			levelDescriptions = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/leveldescrip.txt");
		} catch(FileNotFoundException) {
			Debug.Log("level descriptions not found");
		}

		try {
			levelNames = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/levelnames.txt");
		} catch(FileNotFoundException) {
			Debug.Log("level names not found");
		}
		
		if(levelDescriptions == null || levelNames == null //missing files
			|| levelDescriptions.Length < linesPerLevel * Level.numLevels //insufficient data
			|| levelNames.Length < Level.numLevels) {
			return;
		}
		
		//begin parsing
		this.levelDescriptions = new Dictionary<Level, string>();
		this.levelNames = new Dictionary<Level, string>();
		StringBuilder sb = new StringBuilder();

		for(int stage = 0; stage < Level.numStages; stage++) {

			for(int substage = 0; substage < Level.numSubstages; substage++) {

				Level level = new Level(stage, substage);
				int index = stage * Level.numSubstages + substage;

				for(int i = 0; i < linesPerLevel; i++) {
					sb.Append(levelDescriptions[linesPerLevel * index + i]).Append("\n");
				}

				this.levelDescriptions.Add(level, sb.ToString());
				this.levelNames.Add(level, levelNames[index]);

				sb.Length = 0; //clear stringbuilder in preparation for next set of level descriptions
			}
		}
	}
}
