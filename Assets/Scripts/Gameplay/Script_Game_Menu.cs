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
	
	private Script_Player_Loader playerScript;
	private Level currentLevel;
	private bool paused;
	private bool pauseable;
	private bool completedLevel;
	private Achievement requirements; //'const'

	private Balls ballResources;

	
	// Use this for initialization
	void Start () {
		Color c = background.color;
		c.a = 0.8f; //make background transparent
		background.color = c;

		Time.timeScale = 0;
		
		//assign methods to each menu button
		playerScript = GameObject.Find("Player").GetComponent<Script_Player_Loader>();
		startButton.onClick.AddListener(delegate { startGame(); });
		endButton.onClick.AddListener(delegate { endGame(); });
		nextButton.onClick.AddListener(delegate { nextLevel(); });
		restartButton.onClick.AddListener(delegate { restartGame(); });
		
		endButton.gameObject.SetActive(false);
		restartButton.gameObject.SetActive(false);
		nextButton.gameObject.SetActive(false);
		crossHair.gameObject.SetActive(false);
		
		currentLevel = GameManager.getInstance().getLevel();
		menuText.text = LevelData.getInstance().getLevelName(currentLevel) + "\n\n"
						+ LevelData.getInstance().getLevelDescription(currentLevel);

		crossHair.transform.localScale = SettingsManager.CrosshairSize * Vector3.one;

		paused = false;
		pauseable = false;
		completedLevel = false;
		pause();

		requirements = GameManager.getInstance().getRequirement(currentLevel);
		if(requirements == null) {
			Debug.Log("error parsing achievements");
			requirements = new Achievement(0,0,0,0);
		}

		ballResources = GameObject.Find("Resources").GetComponent<Balls>();
	}
	
	
	// Update is called once per frame
	void Update () {
		if(!paused) {
            displayProgress(); 
        }

        if(InputManager.getInput().buttonDown(Command.Pause) && pauseable) {
            pause();
        }

	}
	

	public void startGame() {
        pauseable = true;
        pause();
        Cursor.visible = false;

        background.gameObject.SetActive(false);
		startButton.gameObject.SetActive(false); //otherwise pressing space activates pause
		endButton.gameObject.SetActive(true);
		restartButton.gameObject.SetActive(true);
		menuText.text = LevelData.getInstance().getLevelName(currentLevel) + "\nPaused";
    }

    public void endGame() {

 		//don't store the save when exiting from the winning screen
    	if(!completedLevel) {
    		playerScript.storeQuickSave();
    	} else {
    		PlayerManager.getInstance().deleteQuicksave(SettingsManager.CurrentPlayer);
    	}

    	playerScript.resetGravity();

		SettingsManager.QuickSaveLoaded = false;
		SceneManager.LoadScene("Scene_Menu");
    }

    public void restartGame() {
    	playerScript.resetGravity();
    	SettingsManager.QuickSaveLoaded = false;
		SceneManager.LoadScene(currentLevel.ToString());
	}
	
	public void nextLevel() {
		playerScript.resetGravity();

		Level nextLevel;
		if(currentLevel.substage == Level.numSubstages - 1) { //this is last level of the stage
			nextLevel = new Level(currentLevel.stage + 1, 0);
			
		} else { //there are further levels in this stage
			nextLevel = new Level(currentLevel.stage, currentLevel.substage + 1);
		}
			
		SettingsManager.QuickSaveLoaded = false;
		SceneManager.LoadSceneAsync(nextLevel.ToString());
	}
	
	private void displayProgress() {

		string player = SettingsManager.CurrentPlayer;
    	int cubies = playerScript.Cubies;
    	int deaths = playerScript.Deaths;
    	float time = playerScript.TimePassed;
    	float scoreMultiplier = ballResources.getScoreMultiplier(playerScript.CurrentBall);

		StringBuilder sb = new StringBuilder();
		sb.Append("Cubies:").Append(cubies).Append('/').Append(requirements.Cubies)
		  .Append("\nTime:").Append(time.ToString("0.00")).Append('/').Append(requirements.TimeString)
		  .Append("\nDeaths:").Append(deaths).Append('/').Append(requirements.Deaths)
		  .Append("\nPoints:").Append(HighScore.calculateScore(cubies,deaths,time,scoreMultiplier)).Append('/').Append(requirements.Points);

		scoringText.text = sb.ToString();
	}

	//when the level ends, process the final score and any acquired achievements
    public void processScoreAchievements() {
    	pause();
    	pauseable = false;
    	completedLevel = true;

    	string player = SettingsManager.CurrentPlayer;
    	int cubies = playerScript.Cubies;
    	int deaths = playerScript.Deaths;
    	float time = playerScript.TimePassed;
    	float scoreMultiplier = ballResources.getScoreMultiplier(playerScript.CurrentBall);
	
		HighScore currentHighScore = new HighScore(player, cubies, deaths, time, scoreMultiplier);
			 
		//returns true if theres a new highscore
		if(ScoreManager.getInstance().setHighScore(currentLevel, currentHighScore)) {
			
			displayScoreAchievements(currentHighScore, requirements, ScoreManager.getInstance().getHighScores(currentLevel));
			
		} else {
			displayScoreAchievements(currentHighScore, requirements, null);
		}
		
		Achievement newRecord = new Achievement(cubies,deaths, time, 
							HighScore.calculateScore(cubies,deaths, time,scoreMultiplier));


		PlayerManager.getInstance().saveRecord(player, newRecord, currentLevel); //save records to player data txt
		
    }

	private void pause() {
        if(paused) {
            hidePauseMenu();
            Time.timeScale = 1;
            paused = false;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        } else {
            showPauseMenu();
            Time.timeScale = 0;
            paused = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        
    }
	
	public void showPauseMenu() {
		background.gameObject.SetActive(true);
		crossHair.gameObject.SetActive(false);
	}
	
	public void hidePauseMenu() {
		background.gameObject.SetActive(false);
		crossHair.gameObject.SetActive(true);
	}

	//when the level is completed, display final achievements and high scores; highscores will be null if there are no new high scores
	public void displayScoreAchievements(HighScore currentScore, Achievement requirements, HighScore[] highscores) {
		
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
		cubiesText.text = "Cubies: " + currentScore.cubies + "/" + requirements.Cubies;
		
		if(currentScore.cubies == requirements.Cubies) {
			cubiesText.fontStyle = FontStyle.Bold;
		}
		
		Text timeText = Instantiate(achievementText, background.rectTransform);
		timeText.rectTransform.anchoredPosition -= Vector2.up * separation * 1;
		timeText.text = "Time: " + currentScore.time.ToString("0.00") + "/" + requirements.TimeString;
		
		if(currentScore.time < requirements.Time) {
			timeText.fontStyle = FontStyle.Bold;
		}

		Text deathsText = Instantiate(achievementText, background.rectTransform);
		deathsText.text = "Deaths: " + currentScore.deaths + "/" + requirements.Deaths;
		deathsText.rectTransform.anchoredPosition -= Vector2.up * separation * 2;
		
		if(currentScore.deaths <= requirements.Deaths) {
			deathsText.fontStyle = FontStyle.Bold;
		}

		Text pointsText = Instantiate(achievementText, background.rectTransform);
		pointsText.rectTransform.anchoredPosition -= Vector2.up * separation * 3;
		int points = currentScore.calculateScore();
		pointsText.text = "Points: " + points + "/" + requirements.Points + " (x" + currentScore.scoreMultiplier.ToString("0.00") + ")";
		
		if(points >=  requirements.Points) {
			pointsText.fontStyle = FontStyle.Bold;
		}
	}
}
