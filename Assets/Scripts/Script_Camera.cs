using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not physically attached to any gameObject, but provides base functionality for setting skyboxes and lighting
public class Script_Camera : MonoBehaviour {
	
	private Light directionalLight;
	private Skyboxes skyboxes;

	//Start() is not used since LevelManager calls for a change in directionalLight before Start() is called
	void Awake () {
		directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
		skyboxes = GameObject.Find("Resources").GetComponent<Skyboxes>();
	}
	
	//set the directional light lighting and skybox; these settings do not affect ambient/environment lighting; you gotta change this directly in editor
	public void setLighting(int stage) { 
				
		switch(stage) {
			case 0: stage0Settings(); break;
			case 1: stage1Settings(); break;
			case 2: stage2Settings(); break;
			case 3: stage3Settings(); break;
			case 4: stage4Settings(); break;
			case 5: stage5Settings(); break;
		}
	}
	
	private void stage0Settings() {
		directionalLight.color = new Color(255f/255, 209f/255, 84f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(6.3f, 282.3f, 0f);
		directionalLight.flare = skyboxes.flare0;
		GetComponent<Skybox>().material = skyboxes.skyBox0;
	}
	private void stage1Settings() {
		directionalLight.color = new Color(203f/255, 201f/255, 161f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(9.1f, 326.2f, 15.67f);
		directionalLight.flare = skyboxes.flare1;
		GetComponent<Skybox>().material = skyboxes.skyBox1;
	}
	private void stage2Settings() {
		directionalLight.color = new Color(208f/255, 208f/255, 208f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(36.9f, 187.1f, 0f);
		directionalLight.flare = skyboxes.flare2;
		GetComponent<Skybox>().material = skyboxes.skyBox2;
	}
	private void stage3Settings() {
		directionalLight.color = new Color(219f/255, 77f/255, 0f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.6f;
		directionalLight.transform.rotation = Quaternion.Euler(2.9f, 357.4f, 0f);
		directionalLight.flare = skyboxes.flare3;
		GetComponent<Skybox>().material = skyboxes.skyBox3;
	}
	private void stage4Settings() {
		directionalLight.color = new Color(169f/255, 182f/255, 176f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.6f;
		directionalLight.transform.rotation = Quaternion.Euler(17.5f, 15.9f, 2.74f);
		directionalLight.flare = skyboxes.flare4;
		GetComponent<Skybox>().material = skyboxes.skyBox4;
	}
	private void stage5Settings() {
		directionalLight.color = new Color(192f/255, 255f/255, 250f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.7f;
		directionalLight.transform.rotation = Quaternion.Euler(24.0f, 204.8f, 0f);
		directionalLight.flare = skyboxes.flare5;
		GetComponent<Skybox>().material = skyboxes.skyBox5;
	}
	private void stage6Settings() {
		directionalLight.color = new Color(191f/255, 255f/255, 233f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.4f;
		directionalLight.transform.rotation = Quaternion.Euler(50.0f, 178.1f, 0f);
		directionalLight.flare = skyboxes.flare6;
		GetComponent<Skybox>().material = skyboxes.skyBox6;
	}
	private void stage7Settings() {
		directionalLight.color = new Color(255f/255, 221f/255, 28f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.7f;
		directionalLight.transform.rotation = Quaternion.Euler(3.3f, 241.1f, 0f);
		directionalLight.flare = skyboxes.flare7;
		GetComponent<Skybox>().material = skyboxes.skyBox7;
	}
}
