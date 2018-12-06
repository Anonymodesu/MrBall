using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Level_Select : MonoBehaviour {

    #pragma warning disable 0649

    [SerializeField]
    private GameObject levelButton, stageButton, levelContainer, stageContainer;

    #pragma warning restore 0649


    private GameObject[] levelButtons;
    private Button[] stageButtons;

    private static readonly Color selectedStageColor = new Color(98f/255, 152f/255, 186f/255, 255f/255);
    private static readonly Color deselectedStageColor = Color.cyan;

	// Use this for initialization
	void Start () {

        //instantiate stage buttons
        stageButtons = new Button[Level.numStages];
        for(int i = 0; i < Level.numStages; i++) {
            stageButtons[i] = Instantiate(stageButton, stageContainer.transform).GetComponentInChildren<Button>();;
            stageButtons[i].GetComponentInChildren<Text>().text = i.ToString();

            int temp = i; //this is needed as the parameter is passed as a reference in switchStages(int)
            stageButtons[i].onClick.AddListener( delegate{ switchStages(temp); });
        }

        //instantiate substage buttons
        levelButtons = new GameObject[Level.numSubstages];
        for(int i = 0; i < Level.numSubstages; i++) {
            levelButtons[i] = Instantiate(levelButton, levelContainer.transform); //share the same parent
        }

        switchStages(GameManager.getInstance().getLevel().stage);
	}
    
    public void switchStages(int stage) {

        //highlight the selected stage button
        foreach(Button button in stageButtons) {
            button.GetComponent<Image>().color = deselectedStageColor;
            button.GetComponentInChildren<Text>().color = Color.black;
        }
        stageButtons[stage].GetComponent<Image>().color = selectedStageColor;
        stageButtons[stage].transform.SetSiblingIndex(0);
        stageButtons[stage].GetComponentInChildren<Text>().color = Color.white;

        //change parameters of all level buttons
        for(int substage = 0; substage < Level.numSubstages; substage++) {
            levelButtons[substage].GetComponent<Script_Menu_Stage_Select_Button>().setLevel(new Level(stage, substage));
            levelButtons[substage].GetComponentInChildren<Text>().text = "Stage " + stage + "-" + substage;
        }
    }
}
