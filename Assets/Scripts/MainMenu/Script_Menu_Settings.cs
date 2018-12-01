using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class Script_Menu_Settings : MonoBehaviour {

	[SerializeField]
	private Slider musicSlider, SFXSlider, cameraSensitivitySlider, crossHairSlider, 
					forwardDistSlider, upDistSlider;

	[SerializeField]
	private InputField nameInputField;

	[SerializeField]
	private ScrollRect namesScrollRect;
	[SerializeField]
	private GameObject playerNameContainer;

	[SerializeField]
	private Toggle shadowToggle, trailsToggle;

	[SerializeField]
	private Transform cameraPreviewOverlay;
	private Transform crosshair;

	private Script_Menu_Camera cameraScript;

	// Use this for initialization
	void Start () {
		cameraScript = Camera.main.GetComponent<Script_Menu_Camera>();
		crosshair = cameraPreviewOverlay.Find("Crosshair");
		initSettings();
	}

	//loads settings saved onto disk
	private void initSettings() {
		musicSlider.value = SettingsManager.MusicVolume;
		SFXSlider.value = SettingsManager.SFXVolume;
		cameraSensitivitySlider.value = SettingsManager.CameraSensitivity;
		shadowToggle.isOn = SettingsManager.DisplayShadows;
		trailsToggle.isOn = SettingsManager.DisplayTrails;
		crossHairSlider.value = SettingsManager.CrosshairSize;
		forwardDistSlider.value = SettingsManager.ForwardDistance;
		upDistSlider.value = SettingsManager.UpwardDistance;

		setMusicVolume();
		setSFXVolume();

		//load player data from file system
		string currentPlayer = SettingsManager.CurrentPlayer;
		nameInputField.text = currentPlayer;

		//read existing achievement files for old players to add to the dropdown
		foreach(string playerPath in Directory.GetFiles(Application.streamingAssetsPath, "*.dat")) {
			string player = Path.GetFileNameWithoutExtension(playerPath);
			createNewPlayer(player);
		}

		//the only time when playerNames does not contain currentPlayer is when currentPlayer has not finished any levels
		//inputField.text has been set above
		setNewPlayer();
	}

	public void restoreDefaults() {
		musicSlider.value = SettingsManager.defaultMusicVolume;
		SFXSlider.value = SettingsManager.defaultSFXVolume;
		cameraSensitivitySlider.value = SettingsManager.defaultCameraSensitivity;
		shadowToggle.isOn = true;
		trailsToggle.isOn = true;
		crossHairSlider.value = SettingsManager.defaultCrosshairSize;
		upDistSlider.value = SettingsManager.defaultUpDist;
		forwardDistSlider.value = SettingsManager.defaultForwardDist;

		setMusicVolume();
		setSFXVolume();
		setCameraSensitivity();
		setShadows();
		setTrails();
		setCrosshairSize();
		setUpwardDistance();
		setForwardDistance();
	}

	public void setMusicVolume() {
		SettingsManager.MusicVolume = musicSlider.value;
		SoundManager.getInstance().MusicVolume = musicSlider.value;
	}

	public void setSFXVolume() {
		SettingsManager.SFXVolume = SFXSlider.value;
		SoundManager.getInstance().SFXVolume = SFXSlider.value;
	}

	public void setCameraSensitivity() {
		SettingsManager.CameraSensitivity = cameraSensitivitySlider.value;
	}

	//creates a new player from the inputfield's text
	public void setNewPlayer() {
		string newPlayer = nameInputField.text;
        if(!name.Equals("")) {

            //search list of options for newPlayer
            GameObject playerButton = null;
            foreach(Transform child in namesScrollRect.content) {
            	if(child.gameObject.name == newPlayer) {
            		playerButton = child.Find("SelectButton").gameObject;
            		break;
            	}
            }

            //add to list if newPlayer does not exist
            if(playerButton == null) {
            	playerButton = createNewPlayer(newPlayer);
            }

            setOldPlayer(playerButton);
        }
	}

	//highlights 'player' from the scrollrect list and assigns it to the inputField
	public void setOldPlayer(GameObject playerButton) {
		foreach(Transform child in namesScrollRect.content) {
			GameObject selectButton = child.Find("SelectButton").gameObject;
			selectButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Normal;
		}

		string player = playerButton.GetComponentInChildren<Text>().text;
		nameInputField.text = player;
		playerButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;

        SettingsManager.CurrentPlayer = player;
	}

	//puts a player name in the scroll rect; returns the player's corresponding button
	private GameObject createNewPlayer(string player) {
		GameObject newPlayer = Instantiate(playerNameContainer, namesScrollRect.content);
		newPlayer.name = player;

		GameObject deleteButton = newPlayer.transform.Find("DeleteButton").gameObject;
		deleteButton.GetComponent<Button>().onClick.AddListener( delegate{ deletePlayer(newPlayer); } );

		GameObject selectButton = newPlayer.transform.Find("SelectButton").gameObject;
		selectButton.GetComponentInChildren<Text>().text = player;
		selectButton.GetComponent<Button>().onClick.AddListener( delegate{ setOldPlayer(selectButton); } );
		return selectButton;
	}

	public void deletePlayer(GameObject playerContainer) {

		Debug.Log("deleting data for " + playerContainer.name);

		switch(namesScrollRect.content.childCount) {
			case 0: //should never happen
				Debug.Log("deleting playerContainer for " + playerContainer.name + " when scroll rect has no children!");
				break;

			case 1: //disallow deleting the last player record;
				Debug.Log("can't delete last record!");
				break;

			default: //set current player to be the next child

				//name of the gameobject corresponds to its representative player
				if(playerContainer.name == nameInputField.text) {
					int nextPlayerIndex = (playerContainer.transform.GetSiblingIndex() + 1) % namesScrollRect.content.childCount;
					Transform nextPlayer = namesScrollRect.content.GetChild(nextPlayerIndex);
					nameInputField.text = nextPlayer.name;
					setOldPlayer(nextPlayer.Find("SelectButton").gameObject);
				}

				//delete everything blyat
				AchievementManager.getInstance().deleteAchievement(playerContainer.name);
				GameManager.getInstance().deleteQuickSave(playerContainer.name);
				Destroy(playerContainer);
				break;
		}
	}

	public void setShadows() {
		SettingsManager.DisplayShadows = shadowToggle.isOn;
	}

	public void setTrails() {
		SettingsManager.DisplayTrails = trailsToggle.isOn;
	}

	public void setCrosshairSize() {
		SettingsManager.CrosshairSize = crossHairSlider.value;
	}

	public void setForwardDistance() {
		SettingsManager.ForwardDistance = forwardDistSlider.value;
	}

	public void setUpwardDistance() {
		SettingsManager.UpwardDistance = upDistSlider.value;
	}

	public void previewCamera() {
		StartCoroutine(previewCameraHelper());
	}

	//add a temporary game camera with a disabled collider to simulate camera movement
	private IEnumerator previewCameraHelper() {
		crosshair.localScale = crossHairSlider.value * Vector3.one;
		Script_Game_Camera tempScript = cameraScript.gameObject.AddComponent<Script_Game_Camera>();
		BoxCollider tempCollider = cameraScript.gameObject.AddComponent<BoxCollider>();
		cameraPreviewOverlay.gameObject.SetActive(true);
		tempCollider.enabled = false;
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		EventSystem.current.enabled = false;

		while(!Input.GetButtonDown("Cancel")) { //wait for cancel button to return to settings menu
			yield return null;
		}

		Destroy(tempScript);
		Destroy(tempCollider);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		cameraPreviewOverlay.gameObject.SetActive(false);

		StartCoroutine(cameraScript.switchMenus(transform));
	}
}