using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_High_Scores : MonoBehaviour {
        
    public Text display;

    // Use this for initialization
    void Start () {
		//Debug.Log(Application.dataPath);        
        List<String> stages = new List<String>();
        for(int i = 0; i < Level.numStages; i++) {
            for(int j = 0; j < Level.numSubstages; j++) {
                stages.Add("Stage " + i + "-" + j);
            }
        }
        this.GetComponent<Dropdown>().AddOptions(stages);
        
        switchStages();
    }
    
    
    
    public void switchStages() { //called in Start(), and the high score menu switch button
        int stage = this.GetComponent<Dropdown>().value; //here, "stage" = currentStage * numSubstages + currentSubstage
        string text = "Name\tCubies\tDeaths\tTime\tScore\n";
		HighScore[] highScores = ScoreManager.getInstance().getHighScores();
        
        int startIndex = stage * ScoreManager.numHighScoresPerSubstage;
        for(int i = 0; i < ScoreManager.numHighScoresPerSubstage; i++) {
            if(highScores[startIndex + i] != null) {
                text += highScores[startIndex + i].display(); //run through each entry of the corresponding stage
            }
        }
        
        display.text = text;
    }
}
