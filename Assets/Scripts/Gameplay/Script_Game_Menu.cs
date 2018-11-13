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
		
		endButton.transform.localScale = Vector3.zero;
		restartButton.transform.localScale = Vector3.zero;
		nextButton.transform.localScale = Vector3.zero;
		crossHair.transform.localScale = Vector3.zero;
		
		currentLevel = GameManager.getInstance().getLevel();
		menuText.text = GameManager.getInstance().getLevelName(currentLevel) + "\n\n"
						+ GameManager.getInstance().getLevelDescription(currentLevel);
	}
	
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	
	public void displayProgress(int cubies, float time, int deaths, Achievement requirements) {
		StringBuilder sb = new StringBuilder();
		sb.Append("Cubies:").Append(cubies).Append('/').Append(requirements.cubies)
		  .Append("\nTime:").Append(time.ToString("F2",CultureInfo.InvariantCulture)).Append('/').Append(requirements.time.ToString("F2",CultureInfo.InvariantCulture))
		  .Append("\nDeaths:").Append(deaths).Append('/').Append(requirements.deaths)
		  .Append("\nPoints:").Append(HighScore.calculateScore(cubies,deaths,(int)time)).Append('/').Append(requirements.points);

		scoringText.text = sb.ToString();
	}
	
	public void finishedLevel(int cubies, float time, int deaths, Achievement required) {
		
	}
	
	public void showPauseMenu() {
		background.transform.localScale = Vector3.one;
		crossHair.transform.localScale = Vector3.zero;
	}
	
	public void hidePauseMenu() {
		background.transform.localScale = Vector3.zero;
		crossHair.transform.localScale = Vector3.one;
	}
	
	public void startGame() {
		background.GetComponent<Transform>().localScale = Vector3.zero;
		startButton.GetComponent<Button>().interactable = false; //otherwise pressing space activates pause
		startButton.transform.localScale = Vector3.zero;
		endButton.transform.localScale = Vector3.one;
		restartButton.transform.localScale = Vector3.one;
		menuText.text = GameManager.getInstance().getLevelName(currentLevel) + "\nPaused";
	}
	
	//when the level is completed, display final achievements and high scores; highscores will be null if there are no new high scores
	public void displayScoreAchievements(int cubies, int deaths, float time, Achievement requirements, HighScore[] highscores) {
		
		nextButton.transform.localScale = Vector3.one; 
		menuText.text = "You win!\n";
		
		if(highscores != null) { //if highscores is null, then the current session did not introduce a new high score
			menuText.text += "New HighScore!\n" + "Name\tCubies\tDeaths\tTime\tScore\n";
			for(int i = 0; i < ScoreManager.numHighScoresPerSubstage; i++) {
				if(highscores[i] != null) {
					menuText.text += highscores[i].display();
				}
			}
		}
		
		int separation = 30;
		Text cubiesText = Instantiate(achievementText, background.rectTransform);
		cubiesText.rectTransform.anchoredPosition -= Vector2.up * separation * 0;
		cubiesText.text = "Cubies: " + cubies + "/" + requirements.cubies;
		
		if(cubies == requirements.cubies) {
			cubiesText.fontStyle = FontStyle.Bold;
		}
		
		Text timeText = Instantiate(achievementText, background.rectTransform);
		timeText.rectTransform.anchoredPosition -= Vector2.up * separation * 1;
		timeText.text = "Time: " + time.ToString("F2",CultureInfo.InvariantCulture) + "/" + requirements.time.ToString("F2",CultureInfo.InvariantCulture);
		
		if((int)time < requirements.time) {
			timeText.fontStyle = FontStyle.Bold;
		}

		Text deathsText = Instantiate(achievementText, background.rectTransform);
		deathsText.text = "Deaths: " + deaths + "/" + requirements.deaths;
		deathsText.rectTransform.anchoredPosition -= Vector2.up * separation * 2;
		
		if(deaths <= requirements.deaths) {
			deathsText.fontStyle = FontStyle.Bold;
		}

		Text pointsText = Instantiate(achievementText, background.rectTransform);
		pointsText.rectTransform.anchoredPosition -= Vector2.up * separation * 3;
		int points = HighScore.calculateScore(cubies,deaths,(int)time);
		pointsText.text = "Points: " + points + "/" + requirements.points;
		
		if(points >=  requirements.points) {
			pointsText.fontStyle = FontStyle.Bold;
		}
	}
}
