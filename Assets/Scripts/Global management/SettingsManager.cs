using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager {

	public const float defaultMusicVolume = 0.5f;
	public const float defaultSFXVolume = 0.5f;
	public const float defaultCameraSensitivity = 2;
	public const string defaultPlayer = "NewPlayer";
	public const int defaultShadowState = 1;
	public const int defaultTrailsState = 1;
	public const float defaultCrosshairSize = 1;
	public const float defaultForwardDist = 2.5f;
	public const float defaultUpDist = 0.7f;



	public static float MusicVolume {
		get { return PlayerPrefs.GetFloat("musicVol", defaultMusicVolume); }
		set { PlayerPrefs.SetFloat("musicVol", value); }
	}

	public static float SFXVolume {
		get { return PlayerPrefs.GetFloat("soundVol", defaultSFXVolume); }
		set { PlayerPrefs.SetFloat("soundVol", value); }
	}

	public static float CameraSensitivity {
		get { return PlayerPrefs.GetFloat("sensitivity", defaultCameraSensitivity); }
		set { PlayerPrefs.SetFloat("sensitivity", value); }
	}

	public static string CurrentPlayer {
		get { return PlayerPrefs.GetString("player", defaultPlayer); }
		set { PlayerPrefs.SetString("player", value); }
	}

	public static bool DisplayShadows {
		get { return PlayerPrefs.GetInt("shadows", defaultShadowState) == 1; }
		set { PlayerPrefs.SetInt("shadows", value ? 1 : 0); }
	}

	public static bool DisplayTrails {
		get { return PlayerPrefs.GetInt("trails", defaultTrailsState) == 1; }
		set { PlayerPrefs.SetInt("trails", value ? 1 : 0); }
	}

	public static float CrosshairSize {
		get { return PlayerPrefs.GetFloat("crosshair", defaultCrosshairSize); }
		set { PlayerPrefs.SetFloat("crosshair", value); }
	}

	public static float ForwardDistance {
		get { return PlayerPrefs.GetFloat("forwardDist", defaultForwardDist); }
		set { PlayerPrefs.SetFloat("forwardDist", value); }
	}

	public static float UpwardDistance {
		get { return PlayerPrefs.GetFloat("upDist", defaultUpDist); }
		set { PlayerPrefs.SetFloat("upDist", value); }
	}

	public static bool QuickSaveLoaded {
		get { return PlayerPrefs.GetInt("save") == 1; }
		set { PlayerPrefs.SetInt("save", value ? 1 : 0); }
	}
}
