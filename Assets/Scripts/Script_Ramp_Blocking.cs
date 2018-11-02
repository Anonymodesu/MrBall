using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Script_Ramp_Blocking : MonoBehaviour {
    const float maxDistance = 2;
    float distance;
    float velocity;

	// Use this for initialization
	void Start () {
		//Transform blockedRamp = GameObject.Find("Rollercoaster/Ramp_Blocked").GetComponent<Transform>();;
        velocity = 0.01f;
        distance = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(Math.Abs(distance) < maxDistance) {
            distance += velocity;
            transform.position += velocity * Vector3.right;
        } else {
            distance = 0;
            velocity *= -1;
        }
        
	}
}
