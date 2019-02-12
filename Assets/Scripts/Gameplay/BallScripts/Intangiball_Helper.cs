using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intangiball_Helper : MonoBehaviour {

	private List<GameObject> contacts;

	public bool Colliding {
		get { return contacts.Count > 0; }
	}

	// Use this for initialization
	void Start () {
		contacts = new List<GameObject>();
	}
	
	void OnTriggerEnter(Collider other) {
		string tag = other.gameObject.tag;
        
    	if (Script_Player_Loader.isPhysical(other.gameObject.tag) && !contacts.Contains(other.gameObject)) {
				 //only add an object if it is not already being contacted
			contacts.Add(other.gameObject);	
        }
	}

	void OnTriggerExit(Collider other) {
        if(Script_Player_Loader.isPhysical(other.gameObject.tag)) {
            contacts.Remove(other.gameObject);
        }
	}
}
