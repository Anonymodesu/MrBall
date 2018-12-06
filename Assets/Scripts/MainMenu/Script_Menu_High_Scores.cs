using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_High_Scores : MonoBehaviour {
        
    #pragma warning disable 0649

    [SerializeField]
    private Text playerText, cubiesText, deathsText, timeText, pointsText;

    #pragma warning restore 0649

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
        
        //when called in the menu, getLevel() returns the last played level
        GetComponent<Dropdown>().value = GameManager.getInstance().getLevel().GetHashCode();
        switchStages();
    }
    
    
    
    public void switchStages() { //called in Start(), and the high score menu switch button
        int levelIndex = this.GetComponent<Dropdown>().value; //here, levelIndex = currentStage * numSubstages + currentSubstage
		HighScore[] highScores = ScoreManager.getInstance().getHighScores();

        playerText.text = "";
        cubiesText.text = "";
        deathsText.text = "";
        timeText.text = "";
        pointsText.text = "";
        
        int startIndex = levelIndex * ScoreManager.numHighScoresPerSubstage;
        for(int i = 0; i < ScoreManager.numHighScoresPerSubstage; i++) {
            HighScore currentHS = highScores[startIndex + i];

            if(currentHS != null) {
               playerText.text += currentHS.name + "\n";
               cubiesText.text += currentHS.cubies + "\n";
               deathsText.text += currentHS.deaths + "\n";
               timeText.text += currentHS.time.ToString("0.00") + "\n";
               pointsText.text += currentHS.calculateScore() + "\n";
            }
        }
        
    }
}
