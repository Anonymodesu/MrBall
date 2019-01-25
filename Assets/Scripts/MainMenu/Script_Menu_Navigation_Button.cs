using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Navigation_Button : Button {
	
    public GameObject destination;

	private Script_Menu_Camera cameraScript;


	// Use this for initialization
	new void Start () {
		base.Start();
		//these figures were found empirically
        cameraScript = GameObject.Find("Menu Camera").GetComponent<Script_Menu_Camera>();

	}
	
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	
	public void MoveMenu() { //only for buttons in the main menu and the back buttons in other menus
		StartCoroutine(cameraScript.switchMenus(destination.transform));
    }

    public void Exit() {
        Application.Quit();
    }

}
