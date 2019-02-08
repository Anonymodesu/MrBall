using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class Script_Quicksave_Button : Script_Menu_Stage_Select_Button {

	private Balls ballResources;

	// Use this for initialization
	new void Start () {
		base.Start();
		ballResources = GameObject.Find("Resources").GetComponent<Balls>();
	}
	
	public override void OnPointerEnter(PointerEventData eventData) {

		Quicksave save = PlayerManager.getInstance().getQuicksave(SettingsManager.CurrentPlayer);
		if(save != null) {
			level = save.level;
			levelText.text = LevelData.getInstance().getLevelName(level);
			
			int cubies = 0;
			foreach(bool collected in save.cubies) {
				if(!collected) {
					cubies++;
				}
			}

			float multiplier = ballResources.getScoreMultiplier(save.ballUsed);
			Achievement current = new Achievement(cubies, save.deaths, save.time, 
													HighScore.calculateScore(cubies, save.deaths, save.time, multiplier));
			Achievement required = GameManager.getInstance().getRequirement(level);
			displayRecords(current, required);

			levelImage.sprite = levelImages.levelImages[level.Index];

		} else {
			level = null;
			levelImage.sprite = null;
			cubiesText.text = "";
			deathsText.text = "";
			timeText.text = "";
			scoreText.text = "";
			levelText.text = "No Quicksave";
		}
		
				
    }


    public override void StartLevel() {
    	if(level != null) { // null if level does not exist
    		SettingsManager.QuickSaveLoaded = true;
        	SceneManager.LoadSceneAsync(level.ToString());
    	}
    	
    }
}
