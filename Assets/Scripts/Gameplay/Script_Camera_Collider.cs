using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//this class exists just to call OnTrigger events for the parent camera since
//parents don't share children's kinematic trigger rigidbody colliders 
public class Script_Camera_Collider : MonoBehaviour {

	private new Transform camera;
	private Transform player;

	void Start () {
		camera = Camera.main.transform;
		player = GameObject.FindWithTag("Player").transform;
	}
	
	void LateUpdate () {

		//ensures that the collider always spans the length between the camera and the ball
		transform.position = (camera.position + player.position) / 2;
		transform.rotation = Quaternion.LookRotation(player.position - camera.position);
		transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 
										(player.position - camera.position).magnitude); //scale of parent should be 1
	}

	void OnTriggerEnter(Collider other) {
		camera.GetComponent<Script_Game_Camera>().onRampEnter(other);
	}

	void OnTriggerExit(Collider other) {
		camera.GetComponent<Script_Game_Camera>().onRampExit(other);
	}
}
