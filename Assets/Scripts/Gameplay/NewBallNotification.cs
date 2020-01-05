using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class NewBallNotification {
	private Image newBallNotification;//the gameobject that is the parent of the whole notification
	private List<BallType> balls;
	private Balls ballResources;

	private Image ballImage;
	private Text ballName;
	private Button nextBallButton;

	private int numPressed; //number of times the next button has been pressed

    public NewBallNotification(List<BallType> balls, Image newBallNotification, Balls ballResources) {
    	this.newBallNotification = newBallNotification;
    	this.balls = balls;
    	this.ballResources = ballResources;

		ballName = newBallNotification.transform.Find("BallName").GetComponent<Text>();
		ballImage = newBallNotification.transform.Find("BallImage").GetComponent<Image>();
		nextBallButton = newBallNotification.transform.Find("NextButton").GetComponent<Button>();

		numPressed = -1;

    }

    public void initiate(Action<bool> menuToggle) {
    	nextBallButton.onClick.AddListener( delegate{ nextBall(menuToggle); });
    	nextBall(menuToggle);
    }

    private void nextBall(Action<bool> menuToggle) {
    	numPressed++;

    	if(numPressed < balls.Count) {
    		BallType currentBall = balls[numPressed];
	    	ballName.text = ballResources.getName(currentBall);
	    	ballImage.sprite = ballResources.getSprite(currentBall);
	    	newBallNotification.gameObject.SetActive(true);
	    	menuToggle(false);

    	} else {
	    	newBallNotification.gameObject.SetActive(false);
			menuToggle(true);
    	}


    }

    
}
