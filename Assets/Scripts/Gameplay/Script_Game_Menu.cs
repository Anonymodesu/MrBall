using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;


//public methods in this script are called by Script_Player
public class Script_Game_Menu : MonoBehaviour {

	//stuff are already loaded into these
	public Image background;
	public Text menuText;
	public Button startButton;
	public Button endButton;
	public Button restartButton;
	public Button nextButton;
	public Text achievementText; 
	public Text crossHair;

	public Text scoringText; //scoringText is not a child of 'background'
	
	private Script_Player playerScript;
	private Level currentLevel;
	
	
	// Use this for initialization
	void Start () {
		Color c = background.color;
		c.a = 0.8f; //make background transparent
		background.color = c;
		
		//assign methods to each menu button
		playerScript = GameObject.Find("Player").GetComponent<Script_Player>();
		startButton.onClick.AddListener(delegate {playerScript.startGame(); });
		endButton.onClick.AddListener(delegate {playerScript.endGame(); });
		nextButton.onClick.AddListener(delegate {playerScript.nextLevel(); });
		restartButton.onClick.AddListener(delegate {playerScript.restartGame(); });
		
		endButton.gameObject.SetActive(false);
		restartButton.gameObject.SetActive(false);
		nextButton.gameObject.SetActive(false);
		crossHair.gameObject.SetActive(false);
		
		currentLevel = GameManager.getInstance().getLevel();
		menuText.text = LevelData.getInstance().getLevelName(currentLevel) + "\n\n"
						+ LevelData.getInstance().getLevelDescription(currentLevel);

		crossHair.transform.localScale = SettingsManager.CrosshairSize * Vector3.one;
	}
	
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	
	public void displayProgress(int cubies, float time, int deaths, Achievement requirements) {


		StringBuilder sb = new StringBuilder();
		sb.Append("Cubies:").Append(cubies).Append('/').Append(requirements.Cubies)
		  .Append("\nTime:").Append(time.ToString("0.00")).Append('/').Append(requirements.TimeString)
		  .Append("\nDeaths:").Append(deaths).Append('/').Append(requirements.Deaths)
		  .Append("\nPoints:").Append(HighScore.calculateScore(cubies,deaths,time)).Append('/').Append(requirements.Points);

		scoringText.text = sb.ToString();
	}
	
	public void finishedLevel(int cubies, float time, int deaths, Achievement required) {
		
	}
	
	public void showPauseMenu() {
		background.gameObject.SetActive(true);
		crossHair.gameObject.SetActive(false);
	}
	
	public void hidePauseMenu() {
		background.gameObject.SetActive(false);
		crossHair.gameObject.SetActive(true);
	}
	
	public void startGame() {
		background.gameObject.SetActive(false);
		startButton.gameObject.SetActive(false); //otherwise pressing space activates pause
		endButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(true);
		menuText.text = LevelData.getInstance().getLevelName(currentLevel) + "\nPaused";
	}
	
	//when the level is completed, display final achievements and high scores; highscores will be null if there are no new high scores
	public void displayScoreAchievements(int cubies, int deaths, float time, Achievement requirements, HighScore[] highscores) {
		
		nextButton.gameObject.SetActive(true); 
		menuText.text = "You win!\n";
		
		if(highscores != null) { //if highscores is null, then the current session did not introduce a new high score
			menuText.text += "New HighScore!\n" + "Name\tCubies\tDeaths\tTime\tScore\n";
			for(int i = 0; i < ScoreManager.numHighScoresPerSubstage; i++) {
				if(highscores[i] != null) {
					menuText.text += highscores[i].display();
				}
			}
		}
		
		int separation = 20;
		Text cubiesText = Instantiate(achievementText, background.rectTransform);
		cubiesText.rectTransform.anchoredPosition -= Vector2.up * separation * 0;
		cubiesText.text = "Cubies: " + cubies + "/" + requirements.Cubies;
		
		if(cubies == requirements.Cubies) {
			cubiesText.fontStyle = FontStyle.Bold;
		}
		
		Text timeText = Instantiate(achievementText, background.rectTransform);
		timeText.rectTransform.anchoredPosition -= Vector2.up * separation * 1;
		timeText.text = "Time: " + time.ToString("0.00") + "/" + requirements.TimeString;
		
		if(time < requirements.Time) {
			timeText.fontStyle = FontStyle.Bold;
		}

		Text deathsText = Instantiate(achievementText, background.rectTransform);
		deathsText.text = "Deaths: " + deaths + "/" + requirements.Deaths;
		deathsText.rectTransform.anchoredPosition -= Vector2.up * separation * 2;
		
		if(deaths <= requirements.Deaths) {
			deathsText.fontStyle = FontStyle.Bold;
		}

		Text pointsText = Instantiate(achievementText, background.rectTransform);
		pointsText.rectTransform.anchoredPosition -= Vector2.up * separation * 3;
		int points = HighScore.calculateScore(cubies,deaths,time);
		pointsText.text = "Points: " + points + "/" + requirements.Points;
		
		if(points >=  requirements.Points) {
			pointsText.fontStyle = FontStyle.Bold;
		}
	}
}
