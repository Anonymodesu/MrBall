using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Menu_Cubie : MonoBehaviour { //for the single cubie in the menu

	// Use this for initialization
	void Start () {
		transform.rotation = transform.rotation * Quaternion.Euler(45,45,45);
        
       
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = transform.rotation * Quaternion.Euler(0,0,60 * Time.deltaTime);
	}
    

}
