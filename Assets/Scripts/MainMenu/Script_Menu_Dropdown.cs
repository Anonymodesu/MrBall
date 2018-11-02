using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Dropdown : MonoBehaviour {

    public GameObject buttonPrefab;

    private GameObject[] stageButtons;    
    private const int startingHeight = 64;
    private const int separationHeight = 30;
    

	// Use this for initialization
	void Start () {
		stageButtons = new GameObject[GameManager.numStages];

        //prepare stages
        List<String> stages = new List<String>();
        for(int i = 0; i < GameManager.numStages; i++) {
            stages.Add("Stage " + i);
        }
        this.GetComponent<Dropdown>().AddOptions(stages);
        
        //instantiate buttons
        for(int i = 0; i < GameManager.numSubstages; i++) {
            stageButtons[i] = Instantiate(buttonPrefab, this.transform.parent); //share the same parent
            
            //modify position
            RectTransform trans = stageButtons[i].GetComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(trans.anchoredPosition.x, startingHeight - separationHeight * i);
            
            //modify text
            changeText(i);
        }
	}
	
	/* Update is called once per frame
	void Update () {
		
	}*/
    
    private void changeText(int substage) {
        string text = "Stage " + this.GetComponent<Dropdown>().value.ToString() + "-" + substage;
        stageButtons[substage].GetComponentInChildren<Text>().text = text;
    }
    
    public void switchStages() {
        for(int i = 0; i < GameManager.numSubstages; i++) {
            changeText(i);
        }
    }
}
