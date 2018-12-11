﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Booster : MonoBehaviour {

	[SerializeField]
	private GameObject lightningEffect;
	private const float lightningDuration = 1;

	private const float boostStrength = 12;
	private const float rotationSpeed = 0.2f;



	void Update() {
		transform.Rotate(transform.forward, rotationSpeed * Time.deltaTime * 60, Space.World);
	}
	
	//booster regions are configured to only collider with the player in Physics settings
	void OnTriggerEnter(Collider other) {
		if(other.isTrigger) { //only interact with the player's trigger collider
			Rigidbody rb = other.GetComponent<Rigidbody>();
			Vector3 dir = Mathf.Sign(Vector3.Dot(rb.velocity, transform.forward)) * transform.forward.normalized;
			rb.AddForce(dir * boostStrength, ForceMode.Impulse);

			other.GetComponent<Script_Player_Trails>().updateColours("Booster");

			SoundManager.getInstance().playSoundFX(SoundFX.Booster);
			StartCoroutine(spawnLightning(other.gameObject));
		}
	}

	private IEnumerator spawnLightning(GameObject target) {
		GameObject instance = Instantiate(lightningEffect);
		
		while(instance.activeSelf) {
			Vector3 velocity = target.GetComponent<Rigidbody>().velocity;
			instance.transform.position = target.transform.position;
			yield return new WaitForFixedUpdate();
		}

		Destroy(instance);
	}

}
