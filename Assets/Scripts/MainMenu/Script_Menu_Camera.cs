using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Menu_Camera : Script_Camera {

    private Quaternion source;
    private Quaternion dest;
    
    private const float rotationTime = 100;
    private float rotationStep;

	// Use this for initialization
	void Start () {
        GameManager.getInstance();
		source = Quaternion.LookRotation(new Vector3(-0.2f, 0, 1));
        dest = source;
        rotationStep = rotationTime;        
	}
	
	// Update is called once per frame
	void Update () {
		if(rotationStep < rotationTime) {
            transform.rotation = Quaternion.Slerp(source, dest, rotationStep / rotationTime);
            
            rotationStep += Time.deltaTime * 60;
            
        } else {
            transform.rotation = dest;
        }
        
        
	}
    
    public void switchMenus(Quaternion source, Quaternion dest) {
        this.source = source;
        this.dest = dest;
        rotationStep = 0;
    }
}
