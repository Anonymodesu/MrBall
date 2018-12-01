﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_Quicksave_Button : Script_Menu_Stage_Select_Button {

	// Use this for initialization
	new void Start () {
		base.Start();
	}
	
	public override void OnPointerEnter(PointerEventData eventData) {

		Quicksave save = GameManager.getInstance().getQuickSave(SettingsManager.CurrentPlayer);
		if(save != null) {
			level = save.level;
			levelText.text = GameManager.getInstance().getLevelName(level);
			
			int cubies = 0;
			foreach(bool collected in save.cubies) {
				if(!collected) {
					cubies++;
				}
			}
			Achievement current = new Achievement(cubies, save.deaths, save.time, 
													HighScore.calculateScore(cubies, save.deaths, save.time));
			Achievement required = AchievementManager.getInstance().getRequirement(level);
			displayRecords(current, required);

			levelImage.sprite = levelImages.levelImages[level.stage * Level.numSubstages + level.substage];

		} else {
			levelImage.sprite = null;
			cubiesText.text = "";
			deathsText.text = "";
			timeText.text = "";
			scoreText.text = "";
			levelText.text = "No Quicksave";
		}
		
				
    }


    public override void StartLevel() {
    	PlayerPrefs.SetInt("save", (int) GameManager.QuickSaveState.LoadSave); //tell program that a quicksave is being loaded
        base.StartLevel();
    }
}
