using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Cubies : MonoBehaviour {

	// Use this for initialization
	void Start () {
		foreach(Transform child in transform) {
            child.rotation = child.rotation * Quaternion.Euler(45,45,45); //tilts the cubies
        }
	}
	
	// Update is called once per frame
	void Update () {
		foreach(Transform child in transform) {
            child.rotation = child.rotation * Quaternion.Euler(0,0,60 * Time.deltaTime);
        }
	}
}
