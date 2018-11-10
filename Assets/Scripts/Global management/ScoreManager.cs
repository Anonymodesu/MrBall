using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

//this class handles file I/O for data retrieval from txt files, as well as storing global constants
//acts like a Factory design pattern for information
public class ScoreManager {
		
	private static readonly string pathName = Application.streamingAssetsPath + "/highscores.money";

    public const int numHighScoresPerSubstage = 5;
    public const int numHighScores = GameManager.numLevels * numHighScoresPerSubstage;
    public const int numFields = 4; //high scores: (name,cubies,deaths,time)
    
	private HighScore[] highScores;
	
	private static ScoreManager instance = null;

	public static ScoreManager getInstance() {
		if(instance == null) {
			instance = new ScoreManager();
		}
		
		return instance;
	}
	
	private ScoreManager() {
		Application.quitting += saveHighScores; //call saveHighScores when the Application.quitting event is raised
		parseHighScores(); //read past high scores and store it in current game session
	}
	
    
    //reads high score file 
    private void parseHighScores() {
        if(File.Exists(pathName)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(pathName, FileMode.Open);
            highScores = (HighScore[]) bf.Deserialize(file);
            file.Close();

        } else {
            highScores = new HighScore[numHighScores];
            Debug.Log("creating new high scores file");
        }

    }
    
    //update high scores list with the new score; return true iff there is a new high score
    public bool setHighScore(Level level, HighScore current) {
        List<HighScore> list = new List<HighScore>();
        //index corresponding to the first highscore in the particular stage/substage
        int startIndex = level.stage * GameManager.numSubstages * numHighScoresPerSubstage + level.substage * numHighScoresPerSubstage;
        
        //consider only the high scores corresponding to the current stage
        for(int i = 0; i < numHighScoresPerSubstage; i++) {
            list.Add(highScores[i + startIndex]);
        }
        list.Add(current); //the score for the current session
        
        list.Sort(new HighScoreComparer());
        
        if(System.Object.ReferenceEquals(list.Last(), current)) {
            return false; //if the last element in the list is the current session's score
            
        } else { //new high score! woo!
			
            for(int i = 0; i < numHighScoresPerSubstage; i++) {
                highScores[i + startIndex] = list[i]; //update highscores for the substage
            }
			return true;
        }
    }
	
	//returns the set of high scores corresponding to the level
	public HighScore[] getHighScores(Level level) {
		HighScore[] levelScores = new HighScore[numHighScoresPerSubstage];
		int startIndex = level.stage * GameManager.numSubstages * numHighScoresPerSubstage + level.substage * numHighScoresPerSubstage;
		for(int i = 0; i < numHighScoresPerSubstage; i++) {
			levelScores[i] = highScores[i + startIndex];
		}
		
		return levelScores;
	}
	
	public HighScore[] getHighScores() {
		return this.highScores;
	}
	
	public void saveHighScores() { //save the stored set of high scores onto disk

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(pathName);

        bf.Serialize(file, highScores);
        file.Close();
	}
	
	
}
