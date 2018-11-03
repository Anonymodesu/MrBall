using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//also called by GameManager
public class SoundManager {

	private GameObject container; //provides a gameobject for audiosources to be attached to
	private AudioSource bgMusic;
	private AudioSource rollSound;
	private AudioSource jumpSound;
	private AudioSource superJumpSound;
		
	private string currentScene;
	private Rigidbody rb;
	
	private static SoundManager instance = null;
	
	public static SoundManager getInstance() {
		if(instance == null) {
			instance = new SoundManager();
		}
		
		return instance;
	}
	/*
	void Awake () {
		
		rollSound = gameObject.AddComponent<AudioSource>();
		jumpSound = gameObject.AddComponent<AudioSource>();
		superJumpSound = gameObject.AddComponent<AudioSource>();
		
	}
	*/
	
	private SoundManager() { //called by LevelManager.OnEnable() to make sure bgMusic is initialised
		container = new GameObject();
		Object.DontDestroyOnLoad(container);
		bgMusic = container.AddComponent<AudioSource>();
	}
	
	/* resets bg music to match the current level
	private void reset() {
		Scene scene = SceneManager.GetActiveScene();
		currentScene = scene.name;
		AudioClip currentBGM = bgMusic.clip;
		AudioClip nextBGM = bgMusic.clip; //to get rid of the "use of unassigned variable" warning

		if(currentScene == "Scene_Menu") { //returning to the menu from a stage does not change the music
			
			if(bgMusic.clip == null) {
				nextBGM = music0;
			}
			
		} else {
			int stage = LevelManager.getLevel(currentScene).stage;
		
			switch(stage) { //background music is dependent on the stage
				case 0: nextBGM = music0; break;
				case 1: nextBGM = music1; break;
				case 2: nextBGM = music2; break;
				case 3: nextBGM = music3; break;
				case 4: nextBGM = music4; break;
				case 5: nextBGM = music5; break;
				case 6: nextBGM = music6; break;
			}
		}
		
		if(nextBGM != currentBGM) { //when the bgm is the same, continue playing
			bgMusic.clip = nextBGM;
			bgMusic.volume = 0.8f;
			bgMusic.loop = true;
			bgMusic.Play();
		}
		
		
	}*/
	
	public void setMusic(int stage) {
		AudioClip nextBGM = null;
		MusicFiles musicFiles = GameObject.Find("Resources").GetComponent<MusicFiles>();
		
		bgMusic.clip = musicFiles.music[stage];
		bgMusic.volume = 0.8f;
		bgMusic.loop = true;
		bgMusic.Play();
	}
}
