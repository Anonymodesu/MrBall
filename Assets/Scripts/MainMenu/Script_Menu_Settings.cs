using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Settings : MonoBehaviour {


	public const float defaultMusicVolume = 0.5f;
	public const float defaultSFXVolume = 0.5f;
	public const float defaultCameraSensitivity = 2;

	[SerializeField]
	private Slider musicSlider, SFXSlider, cameraSensitivitySlider;

	[SerializeField]
	private InputField nameInputField;

	[SerializeField]
	private ScrollRect namesScrollRect;
	[SerializeField]
	private GameObject playerNameContainer;

	// Use this for initialization
	void Start () {
		initSettings();
	}

	private void initSettings() {
		musicSlider.value = PlayerPrefs.GetFloat("musicVol", defaultMusicVolume);
		SFXSlider.value = PlayerPrefs.GetFloat("soundVol", defaultSFXVolume);
		cameraSensitivitySlider.value = PlayerPrefs.GetFloat("sensitivity", defaultCameraSensitivity);

		setMusicVolume();
		setSFXVolume();
		setCameraSensitivity();

		string currentPlayer = PlayerPrefs.GetString("name", "New Player");
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
		musicSlider.value = defaultMusicVolume;
		setMusicVolume();
		SFXSlider.value = defaultSFXVolume;
		setSFXVolume();
		cameraSensitivitySlider.value = defaultCameraSensitivity;
		setCameraSensitivity();
	}

	public void setMusicVolume() {
		PlayerPrefs.SetFloat("musicVol", musicSlider.value);
		SoundManager.getInstance().MusicVolume = musicSlider.value;
	}

	public void setSFXVolume() {
		PlayerPrefs.SetFloat("soundVol", SFXSlider.value);
		SoundManager.getInstance().SFXVolume = SFXSlider.value;
	}

	public void setCameraSensitivity() {
		PlayerPrefs.SetFloat("sensitivity", cameraSensitivitySlider.value);
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
        PlayerPrefs.SetString("name", player);
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
}