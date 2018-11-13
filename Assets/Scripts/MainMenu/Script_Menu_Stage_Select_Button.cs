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

			cubiesText.text = "-/\n " + required.cubies;
			deathsText.text = "-/\n " + required.deaths;
			timeText.text = "-/\n " + required.time;
			scoreText.text = "-/\n " + required.points;

			cubiesText.fontStyle = FontStyle.Normal;
			deathsText.fontStyle = FontStyle.Normal;
			timeText.fontStyle = FontStyle.Normal;
			scoreText.fontStyle = FontStyle.Normal;

		} else { //player has completed this level

			cubiesText.text = current.cubies + "/\n " + required.cubies;
			deathsText.text = current.deaths + "/\n " + required.deaths;
			timeText.text = current.time + "/\n " + required.time;
			scoreText.text = current.points + "/\n " + required.points;

			//check if any of the achievements have been reached
			cubiesText.fontStyle = (current.cubies == required.cubies) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			deathsText.fontStyle = (current.deaths <= required.deaths) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			timeText.fontStyle = (current.time <= required.time) ? FontStyle.BoldAndItalic : FontStyle.Normal;
			scoreText.fontStyle = (current.points >= required.points) ? FontStyle.BoldAndItalic : FontStyle.Normal;
		}
		
    }

	
	public void StartLevel() { //only for buttons in level select menu
        string level = GetComponentInChildren<Text>().text; //the button text corresponds to the level
        AsyncOperation loader = SceneManager.LoadSceneAsync("Scene_" + level);
    }
}
