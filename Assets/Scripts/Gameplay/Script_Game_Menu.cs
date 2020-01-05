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
	public Image newBallNotification;
	public Text levelText;
	public Button startButton;
	public Button endButton;
	public Button restartButton;
	public Button nextButton;
	public UI_Achievement achievementText; 
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
		levelText.text = LevelData.getInstance().getLevelName(currentLevel) + "\n\n"
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
		levelText.text = LevelData.getInstance().getLevelName(currentLevel) + "\nPaused";
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
		currentLevel.Load();
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
		nextLevel.Load();
	}
	
	private void displayProgress() {

		string player = SettingsManager.CurrentPlayer;
    	int cubies = playerScript.Cubies;
    	int deaths = playerScript.Deaths;
    	float time = playerScript.TimePassed;
    	int bonusPoints = playerScript.BonusPoints;
    	float scoreMultiplier = ballResources.getScoreMultiplier(playerScript.CurrentBall);

		StringBuilder sb = new StringBuilder();
		sb.Append("Cubies:").Append(cubies).Append('/').Append(requirements.Cubies)
		  .Append("\nTime:").Append(time.ToString("0.00")).Append('/').Append(requirements.TimeString)
		  .Append("\nDeaths:").Append(deaths).Append('/').Append(requirements.Deaths)
		  .Append("\nPoints:").Append(HighScore.calculateScore(cubies,deaths,time,scoreMultiplier,bonusPoints))
		  .Append('/').Append(requirements.Points);

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
    	int bonusPoints = playerScript.BonusPoints;
    	float scoreMultiplier = ballResources.getScoreMultiplier(playerScript.CurrentBall);
	
		HighScore currentHighScore = new HighScore(player, cubies, deaths, time, scoreMultiplier, bonusPoints);

		Achievement oldRecord = PlayerManager.getInstance().getRecord(player, currentLevel);
		int numOldCubies = PlayerManager.getInstance().getTotalCubies(player);

		//save record to file system
		Achievement newRecord = new Achievement(cubies,deaths, time, 
							HighScore.calculateScore(cubies,deaths, time,scoreMultiplier, bonusPoints));
		Achievement maxRecord = PlayerManager.getInstance().saveRecord(player, newRecord, requirements, currentLevel);
		int numNewCubies = PlayerManager.getInstance().getTotalCubies(player);

		//setHighScore() returns true if theres a new highscore
		bool newHighScoreObtained = ScoreManager.getInstance().setHighScore(currentLevel, currentHighScore);
		HighScore[] highScores = ScoreManager.getInstance().getHighScores(currentLevel);
		displayScoreAchievements(oldRecord, newRecord, requirements, highScores, newHighScoreObtained);

		//prepare to display newly acquired balls and final results of the game session
		List<BallType> newBalls = new List<BallType>();
		foreach(BallType ball in ballResources.newBalls(numOldCubies, numNewCubies)) {
			newBalls.Add(ball);
		}
		NewBallNotification notification = new NewBallNotification(newBalls, newBallNotification, ballResources);
		notification.initiate(toggleFreezeMenu);
    }

    public void toggleFreezeMenu(bool freeze) {
    	background.GetComponent<CanvasGroup>().interactable = freeze;
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
	public void displayScoreAchievements(Achievement oldRecord, Achievement newRecord, Achievement requirements, 
										HighScore[] highscores, bool newHighScore) {
		
		nextButton.gameObject.SetActive(true); 

		//display high scores
		levelText.text = "You win!\n";
		if(newHighScore) {
			levelText.text += "New HighScore!\n";
		}
		
		levelText.text += "Name\tCubies\tDeaths\tTime\tScore\n";
		for(int i = 0; i < ScoreManager.numHighScoresPerSubstage; i++) {
			if(highscores[i] != null) {
				levelText.text += highscores[i].display();
			}
		}
		
		//display achievement progress
		foreach (var eval in Achievement.EvaluateAchievement(requirements, newRecord, oldRecord))  {
			UI_Achievement achievementDisplay = Instantiate(achievementText, levelText.transform.parent);
			achievementDisplay.loadValues(eval);
		}
	}
}
