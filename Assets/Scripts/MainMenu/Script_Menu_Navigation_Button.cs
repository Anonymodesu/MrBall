using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Menu_Navigation_Button : Script_Menu_Button {
	
    [SerializeField]
    private Transform destination;

	private Script_Menu_Camera cameraScript;


	// Use this for initialization
	void Start () {
		base.init();
		//these figures were found empirically
        cameraScript = GameObject.Find("Menu Camera").GetComponent<Script_Menu_Camera>();

	}
	
	/*
	// Update is called once per frame
	void Update () {
		
	}
	*/
	
	public void MoveMenu() { //only for buttons in the main menu and the back buttons in other menus
        StartCoroutine(cameraScript.switchMenus(destination));
    }
}
