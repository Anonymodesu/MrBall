using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Menu_Navigation_Button : Script_Menu_Button {
	
	Quaternion menuRotation;
    Quaternion levelSelectRotation;
    Quaternion highScoresRotation;
	
	public Script_Menu_Camera cameraScript;

	// Use this for initialization
	void Start () {
		base.init();
		//these figures were found empirically
		levelSelectRotation = Quaternion.LookRotation(new Vector3(0.9f, 0, -0.3f));
        menuRotation = Quaternion.LookRotation(new Vector3(-0.2f, 0, 1));
        highScoresRotation = Quaternion.LookRotation(new Vector3(-0.8f,0,-0.6f));
	}
	
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	
	public void MoveMenu() { //only for buttons in the main menu and the back buttons in other menus
        string currentMenu = transform.parent.name;
        
        switch(currentMenu) {
            case "Level Select": //only the back button switches menus in the level select menu
                cameraScript.switchMenus(levelSelectRotation, menuRotation);
                break;
                
            case "Menu": //at main menu
                string destMenu = transform.name;
                
                switch(destMenu) {
                    case "LevelSelectButton":
                        cameraScript.switchMenus(menuRotation, levelSelectRotation);
                        break;
                    
                    case "HighScoresButton":
                        cameraScript.switchMenus(menuRotation, highScoresRotation);
                        break;
                }
                
                break;
                
            case "High Scores": //only the back button switches menus in the high scores menu
                cameraScript.switchMenus(highScoresRotation, menuRotation);
                break;
        }
    }
}
