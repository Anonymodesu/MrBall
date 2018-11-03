using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager { //a singleton
	
		
	public const int numSubstages = 5;
    public const int numStages = 6;
	public const int numLevels = numStages * numSubstages;
	
	private Level currentLevel = null; //stores the level that was just loaded
	
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
	}
	
	~GameManager() {
		SceneManager.sceneLoaded -= changedScene;
	}
	
/*
	void OnEnable () { //OnEnable is guaranteed to be called before any Start() is called; to ensure that no references to 2nd game managers are made
		//also destroys secondary achievement, sound and score managers
	
		if(numInstances == 0) {
			numInstances++;
			DontDestroyOnLoad(this.gameObject); //object is preserved across stages
			
			soundScript = gameObject.GetComponent<SoundManager>();
			soundScript.init();
			soundScript.setMusic(0);
			currentLevel = new Level(0,0); //pretend that the first level was loaded
			SceneManager.sceneLoaded += changedScene; //add changedScene() as a listener to new scenes being loaded
			
		} else if (numInstances == 1) {
			Destroy(this.gameObject); //only have 1 gameObject
			
		} else {
			Debug.Log("more than 1 game manager!"); //should never happen
		}
		
		
						//Debug.Log(this.gameObject.GetInstanceID());

	}
	
	void OnDisable() {
		SceneManager.sceneLoaded -= changedScene;
	}
*/
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
	public static Level getLevel(string sceneName) {
		string temp = sceneName.Split(new char[] {' '})[1]; //get second term of the scene name
        string[] temp2 = temp.Split(new char[] {'-'});
        
        int stage = Int32.Parse(temp2[0]); //stages and substages start indexing from 0
        int subStage = Int32.Parse(temp2[1]);
		
		return new Level(stage,subStage);
	}
	
	//returns the flavour text displayed at the beginning of each level
	public static string getLevelDescription(Level level) {
		
		int linesPerLevel = 5; //each description in the txt file is allowed 5 lines
		string[] lines = null;
		
		try {
			lines = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/leveldescrip.txt");
			
		} catch(FileNotFoundException) {
			Debug.Log("level descriptions not found");
		}
		
		if(lines == null) {
			return "";
			
		} else {
			string ret = "";
			int startIndex = level.stage * numSubstages * linesPerLevel + level.substage * linesPerLevel;
				
			for(int i = 0; i < linesPerLevel; i++) {
				ret += lines[startIndex + i] + "\n";
			}
			
			return ret;
		}
		
	}
	
	
}
