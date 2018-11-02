using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Moving_Array : MonoBehaviour {
    const float farLeft = 0.321f;
    const float farRight = 2.828f;
    int numBlocks; 
    const float speed = 2;
    float[] velocities;
    
	// Use this for initialization
	void Start () {
		numBlocks = transform.childCount;
        velocities = new float[numBlocks];
        
        for(int i = 0; i < numBlocks; i++) {
            velocities[i] = speed;
        }
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < numBlocks; i++) {
            Transform current = transform.GetChild(i);
            
            if(current.localPosition.z > farRight) {
                velocities[i] = -speed;
            } else if(current.localPosition.z < farLeft) {
                velocities[i] = speed;
            }
            
            //Debug.Log(current.position.z);
            
            current.position += Vector3.forward * velocities[i] * Time.deltaTime;
        }
	}
}
