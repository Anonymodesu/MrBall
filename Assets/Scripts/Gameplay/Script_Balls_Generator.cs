using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Balls_Generator : MonoBehaviour {

	public string mainBalls;
	public string normalBalls;
	public string fastBalls;
	public string perpendicularBalls;
	public string bouncyBalls;
	
	public float ballRadius;
	public float ballSeparation;
	public int numRow; //balls per row
	public int numCol;
	
	public GameObject normalBall;
	public GameObject fastBall;
	public GameObject perpendicularBall;
	public GameObject bouncyBall;
	
	private GameObject[] balls;

	//in awake so that Script_OutOfBounds includes the balls in the bounds checking.
	//also destroys balls created using the inspector button;
	//this ensures that balls use the most recent prefab
	void Awake() {
		foreach(Transform child in transform) {
			Destroy(child.gameObject);
		}
		makeBalls();
	}

	//Construct a grid of balls based on specs defined by public variables
	//See Script_Balls_Generator_Helper
	public void makeBalls () {
		
		GameObject mainBall = null; //get rid of use of unassigned var error
		balls = new GameObject[numCol * numRow];
			
		switch(mainBalls) {
			case "normal": mainBall = normalBall; break;
			case "fast": mainBall = fastBall; break;
			case "bouncy": mainBall = bouncyBall; break;
			case "perpendicular": mainBall = perpendicularBall; break;
			default: Debug.Log("invalid main ball"); mainBall = normalBall; break;
		}
		
		HashSet<int> normalPlacement = findPlacements(normalBalls);
		HashSet<int> fastPlacement = findPlacements(fastBalls);
		HashSet<int> bouncyPlacement = findPlacements(bouncyBalls);
		HashSet<int> perpendicularPlacement = findPlacements(perpendicularBalls);
		
		for(int i = 0; i < numRow; i++) {
			for(int j = 0; j < numCol; j++) {
				int index = i * numCol + j;
				
				GameObject newBall = null;
				
				//choose which ball to be placed at this position
				if(normalPlacement.Contains(index)) {
					newBall = normalBall;
					
				} else if(fastPlacement.Contains(index)) {
					newBall = fastBall;
					
				} else if(perpendicularPlacement.Contains(index)) {
					newBall = perpendicularBall;
					
				} else if(bouncyPlacement.Contains(index)) {
					newBall = bouncyBall;
					
				} else { //no special ball
					newBall = mainBall;
				}
				
				balls[index] = Instantiate(newBall, this.transform) as GameObject;
				balls[index].transform.localScale = new Vector3(ballRadius,ballRadius,ballRadius);
				balls[index].transform.localPosition = new Vector3(ballSeparation * i, 0, ballSeparation * j);
			}
		}
	}
	
	//parses the input from the public field
	private HashSet<int> findPlacements(string line) {
		string[] temp =  line.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
		HashSet<int> placements = new HashSet<int>();
		
		foreach(string str in temp) {
			int current;
			
			if(Int32.TryParse(str, out current)) {
				placements.Add(current);
				
			} else {
				Debug.Log("error parsing balls generator placements: " + line + " . at " + str);
			}
		}
		
		return placements;
	}
}
