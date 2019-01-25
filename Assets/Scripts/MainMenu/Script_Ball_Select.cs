using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Script_Ball_Select : Button {

	private Balls ballResources;
	private BallType currentType;
	private Text ballNameField, ballDescripField, ballPowersField;


	private static readonly Color deselectedColour = Color.white;
	private static readonly Color selectedColour = Color.red;

	// Awake() instead of Start() since ballResources needs to be initialised before updateBall is called by Script_Menu_Settings
	new void Awake () {
		base.Awake();

		ballResources = GameObject.Find("Resources").GetComponent<Balls>();
		currentType = BallType.None;
		ballNameField = GameObject.Find("BallNameText").GetComponent<Text>();
		ballDescripField = GameObject.Find("BallDescriptionText").GetComponent<Text>();
		ballPowersField = GameObject.Find("BallPowersText").GetComponent<Text>();
	}

	public void updateBall(BallType type) {
		GetComponent<Image>().sprite = ballResources.getSprite(type);
		currentType = type;
		
		if(type == BallType.None) { //'none' indicates that this ball has not been unlocked yet
			interactable = false; //this property is part of the button class
		} else {
			interactable = true;
		}
	}

	public void setBall() {
		SettingsManager.CurrentBall = currentType;
		
		foreach(Transform ballButton in transform.parent) {
			ballButton.GetComponent<Image>().color = deselectedColour;
		}
		GetComponent<Image>().color = selectedColour;

		//update descriptive fields
		ballNameField.text = ballResources.getName(currentType);
		ballDescripField.text = ballResources.getDescription(currentType);
		ballPowersField.text = ballResources.getPowers(currentType);
	}
}
