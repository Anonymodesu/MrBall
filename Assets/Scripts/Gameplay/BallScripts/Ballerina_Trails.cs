using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ballerina_Trails : Enabled_Trails {

	private const float rotationSpeed = 600;

	//trails A and B rotate in opposite direction
	private ParticleSystem ballerinaTrailsA, ballerinaTrailsB;
	private Script_Game_Camera cameraScript;
	private float rotationAmount;

	public Ballerina_Trails(Script_Player_Loader playerScript) : base(playerScript) {
		ParticleSystem original = GameObject.Find("Resources").GetComponent<Balls>().BallerinaEffect.GetComponent<ParticleSystem>();
		ballerinaTrailsA = UnityEngine.Object.Instantiate(original);
		ballerinaTrailsB = UnityEngine.Object.Instantiate(original);
		cameraScript = GameObject.FindWithTag("MainCamera").GetComponent<Script_Game_Camera>();
		rotationAmount = 0;
	}

	public override void processTrails() {
		base.processTrails();

		//update position and transform regardless of whether ballerina is spinning
		ballerinaTrailsA.transform.position = playerScript.transform.position;
		ballerinaTrailsB.transform.position = playerScript.transform.position;
		
		ballerinaTrailsA.transform.rotation = Quaternion.AngleAxis(rotationAmount, cameraScript.YAxis)
												* Quaternion.FromToRotation(Vector3.up, cameraScript.YAxis);
		ballerinaTrailsB.transform.rotation = Quaternion.AngleAxis(-rotationAmount, cameraScript.YAxis)
												* Quaternion.FromToRotation(Vector3.up, cameraScript.YAxis);
		rotationAmount += (rotationSpeed * Time.deltaTime) % 360;

		//update color (similar to Enabled_Trails.processColours())
		float speed = 60 * Time.deltaTime;
		var main = ballerinaTrailsA.main;
		Color nextColor = Color.Lerp(main.startColor.color, mainColour, colourChangeSpeed * speed);
		main.startColor = nextColor;
		main = ballerinaTrailsB.main;
		main.startColor = nextColor;
	}



	public void startBallerinaTrails() {
		ballerinaTrailsA.Play();
		ballerinaTrailsB.Play();
	}

	public void stopBallerinaTrails() {
		ballerinaTrailsA.Stop();
		ballerinaTrailsB.Stop();
	}
}
