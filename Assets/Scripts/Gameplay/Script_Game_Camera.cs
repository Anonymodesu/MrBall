using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Game_Camera : Script_Camera {
    float mouseSensitivity;
    GameObject player;
    float positionOffset;
	Vector3 rotation; // vector representing an Euler rotation in the standard basis; 
	
	Vector3 xAxis;
	Vector3 yAxis;
	Vector3 zAxis; //the current coordinate system being used, relevant when gravity changes
		
	//upper half rotation is from 0 at the middle down to -90 at the top
	private const float upperLimit = -89; 
	private const float lowerLimit = 89;
	//lower half rotation is from 0 at middle down to 90 at the bottom	

    // Use this for initialization
    void Start () {
        mouseSensitivity = 2;
        player = GameObject.FindWithTag("Player");
        positionOffset = 1;
		rotation = Vector3.zero;
		updateAxes();
        transform.position = GameObject.Find("Ramp_Start").GetComponent<Transform>().position + Vector3.up;
    }
    
    // Update is called once per frame
    void LateUpdate () {
		
		Debug.DrawLine(player.transform.position,player.transform.position+xAxis,Color.red);
		Debug.DrawLine(player.transform.position,player.transform.position+yAxis,Color.blue);
		Debug.DrawLine(player.transform.position,player.transform.position+zAxis,Color.black);
		

			//deltatime is still required to freeze camera during pause
		rotation.x -= mouseSensitivity*Input.GetAxis("Mouse Y") * Time.deltaTime * 60;
		rotation.y += mouseSensitivity*Input.GetAxis("Mouse X") * Time.deltaTime * 60; 
		rotation.z = 0;
		
		//do not allow rotation past the limits
		if(rotation.x < upperLimit && rotation.x > -180) {
			rotation.x = upperLimit;
			
		} else if(rotation.x > lowerLimit && rotation.x < 180) {
			rotation.x = lowerLimit;
		} 
		
		if(rotation.y > 360 || rotation.y < -360) {
			rotation.y = 0;
		}
		
		Vector3 temp = Quaternion.AngleAxis(rotation.y, yAxis) * Quaternion.AngleAxis(rotation.x, xAxis) * zAxis;
		transform.rotation = Quaternion.LookRotation(temp, yAxis);
		
		transform.position = player.transform.position //transform.forward is always normalized
							+ (yAxis * 0.7f - transform.forward * 2) * positionOffset; //position the camera behind the ball 
		
    }
	
	public void updateAxes() { //called when a change in gravity is registered by Script_Player

		yAxis = -Physics.gravity.normalized;
		xAxis = Vector3.Cross(yAxis, transform.forward).normalized;
		
		while(xAxis.Equals(Vector3.zero)) { //almost never happens
			xAxis = Vector3.Cross(yAxis, Random.insideUnitSphere).normalized;
		}
		zAxis = Vector3.Cross(xAxis, yAxis).normalized;

		/* makes the camera face what the camera was previously facing
		float xRotation = Vector3.SignedAngle(zAxis, transform.forward, xAxis);
		float yRotation = Vector3.SignedAngle(zAxis, transform.forward, yAxis);
		rotation = new Vector3(xRotation, yRotation, 0);
		*/

		rotation = Vector3.zero;
	}
	
	// returns the projection of the camera's current forward facing direction onto the x-z plane in the current coordinate basis
	public Vector3 forwardVector() {
		return Vector3.ProjectOnPlane(transform.forward, yAxis);
	}
	
	// returns the projection of camera's current right facing direction onto the x-z plane in the current coordinate basis
	public Vector3 rightVector() {
		return Vector3.ProjectOnPlane(transform.right, yAxis);
	}
	
	
}
