using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Player_Name : MonoBehaviour {
 

    void Start() {
        this.GetComponent<InputField>().text = PlayerPrefs.GetString("name", "New Player");
		
		//cache achievements for the current player
		Script_Menu_Stage_Select_Button.setAchievements(AchievementManager.getInstance().getAchievements(PlayerPrefs.GetString("name", "New Player")));
    }

	public void updateName() {
        string name = this.GetComponent<InputField>().text;
        if(!name.Equals("")) {
            PlayerPrefs.SetString("name", name);
        }
		
		//also update cached achievements for the new player name
		Script_Menu_Stage_Select_Button.setAchievements(AchievementManager.getInstance().getAchievements(PlayerPrefs.GetString("name", "New Player")));
    }
}
