using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Script_Menu_Stage_Select_Button : Script_Menu_Button {
	
	private static Achievement[] achievements;
	private static Text achievementsText;

	// Use this for initialization
	void Start () {
		base.init();
		GetComponent<Button>().onClick.AddListener(delegate {StartLevel(); });
		achievementsText = GameObject.Find("AchievementsText").GetComponent<Text>();
	}

	
	public override void OnPointerEnter(PointerEventData eventData) {
		base.OnPointerEnter(eventData); //move the cubie next to the button next to the button
		
		Level level = GameManager.getInstance().getLevel(GetComponentInChildren<Text>().text); //text in button corresponds to the level
		
		Achievement current = achievements[level.stage * GameManager.numSubstages + level.substage]; //substages start indexing from 1
		
		string cubies;
		string deaths;
		string time;
		string score;
		
		if(current == null) { //player has not completed this level
			cubies = " -";
			deaths = " -";
			time = " -";
			score = " -";
									  
		} else { //player has completed this level
			cubies = current.cubies.ToString();
			deaths = current.deaths.ToString();
			time = current.time.ToString();
			score = current.points.ToString();
		}
		
		//show the current player's records so far
		StringBuilder sb = new StringBuilder();
		achievementsText.text = sb.Append("Records:\n")
								  .Append("\nCubies:").Append(cubies)
								  .Append("\nDeaths:").Append(deaths)
								  .Append("\nTime:").Append(time)
								  .Append("\nScore:").Append(score).ToString();
		
		
    }
	
	public static void setAchievements(Achievement[] achievements) {
		Script_Menu_Stage_Select_Button.achievements = achievements;
	}
	
	public void StartLevel() { //only for buttons in level select menu
        string level = GetComponentInChildren<Text>().text; //the button text corresponds to the level
        AsyncOperation loader = SceneManager.LoadSceneAsync("Scene_" + level);
    }
}
