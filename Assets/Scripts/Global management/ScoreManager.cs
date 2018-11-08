using System.Text;
using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;
using UnityEngine;

//this class handles file I/O for data retrieval from txt files, as well as storing global constants
//acts like a Factory design pattern for information
public class ScoreManager {
		
	
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
		string[] lines;

		try {
			lines =  System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/highscores.txt");

		} catch(FileNotFoundException) {
			Debug.Log("file not found; resetting scores");
			resetScores();
			lines = System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/highscores.txt");
		}
			        
        //parse the file for validity
        if(lines.Length != numHighScores) { //check number of lines
            Debug.Log("file has incorrect number of lines. resetting scores");
            resetScores();
            parseHighScores();//reread highscores.txt

        } else {
			highScores = new HighScore[numHighScores];
                        
            for(int i = 0; i < numHighScores; i++) {
                
                string current = lines[i];
                
                if(current.Equals("")) { //empty record
                    highScores[i] = null;
                    continue;
                    
                } else {
                    string[] elements = current.Split(new char[] {','});
                    
                    if(elements.Length != numFields) { //incorrect number of fields
                        Debug.Log("invalid entry on line " + i.ToString() + ". resetting scores");
                        resetScores();
                        parseHighScores();//reread highscores.txt
                        break;
                        
                    } else { //parse a particular entry
                        string name = elements[0];
                        int cubies;
                        int deaths;
                        int time;
                        
                        if(Int32.TryParse(elements[1], out cubies) && //parse each field in the entry
                           Int32.TryParse(elements[2], out deaths) && 
                           Int32.TryParse(elements[3], out time)) {
                            
                            highScores[i] = new HighScore(name, cubies, deaths, time);
                            
                        } else { //invalid field(s)
                            Debug.Log("invalid entry on line " + i.ToString() + ". resetting scores");
                            resetScores();
                            parseHighScores();//reread highscores.txt
                            break;
                        }
                    }
                }
            }
        }
    }
    
    private void resetScores() { //generate an empty high scores record
        StringBuilder emptyScores = new StringBuilder();

        for(int i = 0; i < GameManager.numStages; i++) {
            for(int j = 0; j < GameManager.numSubstages; j++) {
                for(int k = 0; k < ScoreManager.numHighScoresPerSubstage; k++) {
                    emptyScores.Append(/*"jackson," + i + "," + (j+100) + "," + k + */"\n"); //GET RID OF THESE LATER
                }
            }
        }
        
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/highscores.txt", emptyScores.ToString());
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
		StringBuilder sb = new StringBuilder();
        foreach(HighScore score in highScores) {
            if(score != null) {
                sb.Append(score.Name()).Append(',')
					.Append(score.Cubies()).Append(',')
                    .Append(score.Deaths()).Append(',')
                    .Append(score.Time());
					
				//Debug.Log(score.display());
            }
            sb.Append('\n');
        }
            
        System.IO.File.WriteAllText(Application.streamingAssetsPath + "/highscores.txt", sb.ToString()); //save high scores to txt file
	}
	
	
}
