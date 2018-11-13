using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class Script_Menu_Stage_Select_Button : Script_Menu_Button {

	private Text cubiesText, deathsText, timeText, scoreText, levelText;

	// Use this for initialization
	void Start () {
		base.init();
		GetComponent<Button>().onClick.AddListener(delegate {StartLevel(); });

		cubiesText = GameObject.Find("CubiesText").GetComponent<Text>();
		deathsText = GameObject.Find("DeathsText").GetComponent<Text>();
		timeText = GameObject.Find("TimeText").GetComponent<Text>();
		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		levelText = GameObject.Find("LevelName").GetComponent<Text>();
	}

	
	public override void OnPointerEnter(PointerEventData eventData) {
		base.OnPointerEnter(eventData); //move the cubie next to the button next to the button
		
		Level level = GameManager.getInstance().getLevel(GetComponentInChildren<Text>().text); //text in button corresponds to the level
		levelText.text = GameManager.getInstance().getLevelName(level);
		
		Achievement current = AchievementManager.getInstance().getAchievement(PlayerPrefs.GetString("name", "New Player"), level);
		Achievement required = AchievementManager.getInstance().getRequirement(level);
		
		if(current == null) { //player has not completed this level

			cubiesText.text = "-/\n" + required.Cubies;
			deathsText.text = "-/\n" + required.Deaths;
			timeText.text = "-/\n" + required.TimeString;
			scoreText.text = "-/\n" + required.Points;

			cubiesText.fontStyle = FontStyle.Normal;
			deathsText.fontStyle = FontStyle.Normal;
			timeText.fontStyle = FontStyle.Normal;
			scoreText.fontStyle = FontStyle.Normal;

		} else { //player has completed this level

			cubiesText.text = current.Cubies + "/\n" + required.Cubies;
			deathsText.text = current.Deaths + "/\n" + required.Deaths;
			timeText.text = current.TimeString + "/\n" + required.TimeString;
			scoreText.text = current.Points + "/\n" + required.Points;

			//check if any of the achievements have been reached
			cubiesText.fontStyle = (current.Cubies == required.Cubies) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			deathsText.fontStyle = (current.Deaths <= required.Deaths) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			timeText.fontStyle = (current.Time <= required.Time) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			scoreText.fontStyle = (current.Points >= required.Points) ? FontStyle.BoldAndItalic : FontStyle.Normal;
		}
		
    }

	
	public void StartLevel() { //only for buttons in level select menu
        string level = GetComponentInChildren<Text>().text; //the button text corresponds to the level
        AsyncOperation loader = SceneManager.LoadSceneAsync("Scene_" + level);
    }
}
