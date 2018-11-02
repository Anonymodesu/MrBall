using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not physically attached to any gameObject, but provides base functionality for setting skyboxes and lighting
public class Script_Camera : MonoBehaviour {
	
	private Light directionalLight;

	
	public Material skyBox0;
	public Material skyBox1;
	public Material skyBox2;
	public Material skyBox3;
	public Material skyBox4;
	
	//Start() is not used since LevelManager calls for a change in directionalLight before Start() is called
	void Awake () {
		directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
	}
	
	//set the directional light lighting and skybox; these settings do not affect ambient/environment lighting; you gotta change this directly in editor
	public void setLighting(int stage) { 
		
		Material skyBox = null;
		switch(stage) {
			case 0: skyBox = skyBox0; stage0Settings(); break;
			case 1: skyBox = skyBox1; stage1Settings(); break;
			case 2: skyBox = skyBox2; stage2Settings(); break;
			case 3: skyBox = skyBox3; stage3Settings(); break;
			case 4: skyBox = skyBox4; stage4Settings(); break;
		}
		this.gameObject.GetComponent<Skybox>().material = skyBox;
	}
	
	private void stage0Settings() {
		directionalLight.color = new Color(255f/255, 209f/255, 84f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(11.34f, 278.42f, 0.50f);
	}
	private void stage1Settings() {
		directionalLight.color = new Color(203f/255, 201f/255, 161f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(33.33f, 318.83f, 15.67f);
	}
	private void stage2Settings() {
		directionalLight.color = new Color(208f/255, 208f/255, 208f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.5f;
		directionalLight.transform.rotation = Quaternion.Euler(65.01f, 137.74f, 272.42f);
	}
	private void stage3Settings() {
		directionalLight.color = new Color(219f/255, 77f/255, 0f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.6f;
		directionalLight.transform.rotation = Quaternion.Euler(13.62f, 354.74f, 0.85f);
	}
	private void stage4Settings() {
		directionalLight.color = new Color(169f/255, 182f/255, 176f/255); //max value is 1 for each r,g,b argument
		directionalLight.intensity = 0.6f;
		directionalLight.transform.rotation = Quaternion.Euler(21.44f, 14.22f, 2.74f);
	}
}
