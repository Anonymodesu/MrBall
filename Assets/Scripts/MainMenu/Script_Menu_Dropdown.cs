using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Dropdown : MonoBehaviour {

    public GameObject buttonPrefab;

    private GameObject[] stageButtons;    
    private const int startingHeight = -64;
    private const int separationHeight = 30;
    

	// Use this for initialization
	void Start () {
		stageButtons = new GameObject[Level.numStages];

        //prepare stages
        List<String> stages = new List<String>();
        for(int i = 0; i < Level.numStages; i++) {
            stages.Add("Stage " + i);
        }
        this.GetComponent<Dropdown>().AddOptions(stages);
        
        //instantiate buttons
        for(int i = 0; i < Level.numSubstages; i++) {
            stageButtons[i] = Instantiate(buttonPrefab, GameObject.Find("LevelSelectButtons").transform); //share the same parent
        }

        switchStages();
	}
	
	/* Update is called once per frame
	void Update () {
		
	}*/
    
    public void switchStages() {
        int stage = GetComponent<Dropdown>().value;
        for(int substage = 0; substage < Level.numSubstages; substage++) {
            stageButtons[substage].GetComponent<Script_Menu_Stage_Select_Button>().setLevel(new Level(stage, substage));
            stageButtons[substage].GetComponentInChildren<Text>().text = "Stage " + stage + "-" + substage;
        }
    }
}
