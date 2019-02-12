using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SoundFX : int {Roll = 0, Collision = 1, YellowJump = 2, OrangeJump = 3, NormalJump = 4, Checkpoint = 5, Cubie = 6,
								Win = 7, Gravity = 8, Booster = 9, UsainBowl = 10, BowlVaulter = 11}

//also called by GameManager
public class SoundManager {

	private GameObject container; //provides a gameobject for audiosources to be attached to
	private AudioSource bgMusic;
	private AudioSource soundFX;
	
	private static SoundManager instance = null;

	private MusicFiles soundFiles = null;

	private float[] soundFXVolume;

	public float SFXVolume {
		get {
			return soundFX.volume;
		}

		set {
			if(value < 0 || value > 1) {
				Debug.Log("sound volume is out of bounds");
			}

			soundFX.volume = value;
		}
	}
	
	public float MusicVolume {
		get {
			return bgMusic.volume;
		}

		set {
			if(value < 0 || value > 1) {
				Debug.Log("music volume is out of bounds");
			}

			bgMusic.volume = value;
		}
	}

	public static SoundManager getInstance() {
		if(instance == null) {
			instance = new SoundManager();
		}
		
		return instance;
	}
	
	private SoundManager() { //called by LevelManager.OnEnable() to make sure bgMusic is initialised
		container = new GameObject();
		UnityEngine.Object.DontDestroyOnLoad(container);
		bgMusic = container.AddComponent<AudioSource>();
		soundFX = container.AddComponent<AudioSource>();

		bgMusic.volume = SettingsManager.MusicVolume;
		soundFX.volume = SettingsManager.SFXVolume;

		//array length = total number of enum values defined
		soundFXVolume = new float[Enum.GetNames(typeof(SoundFX)).Length];
		soundFXVolume[(int) SoundFX.Collision] = 0.4f;
		soundFXVolume[(int) SoundFX.NormalJump] = 1f;
		soundFXVolume[(int) SoundFX.YellowJump] = 1f;
		soundFXVolume[(int) SoundFX.OrangeJump] = 0.6f;
		soundFXVolume[(int) SoundFX.Checkpoint] = 1f;
		soundFXVolume[(int) SoundFX.Cubie] = 0.5f;
		soundFXVolume[(int) SoundFX.Win] = 0.5f;
		soundFXVolume[(int) SoundFX.Gravity] = 1f;
		soundFXVolume[(int) SoundFX.Booster] = 0.8f;
		soundFXVolume[(int) SoundFX.UsainBowl] = 0.8f;
		soundFXVolume[(int) SoundFX.BowlVaulter] = 0.8f;
	}
	
	private MusicFiles getSoundFiles() {
		if(soundFiles == null) { //new set of soundfiles for each scene
			soundFiles = GameObject.Find("Resources").GetComponent<MusicFiles>();
		} 

		return soundFiles;
	}
	
	public void setMusic(int stage) {
		bgMusic.clip = getSoundFiles().music[stage];
		bgMusic.loop = true;
		bgMusic.Play();
	}

	//use default assigned volume
	public void playSoundFX(SoundFX sound) {
		playSoundFX(sound, soundFXVolume[(int) sound]);
	}

	public void playSoundFX(SoundFX sound, float volume) {
		soundFX.PlayOneShot(getSoundFiles().soundFX[(int) sound], volume);
	}
}
