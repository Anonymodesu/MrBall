using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Player_Name : MonoBehaviour {
 

    void Start() {
        this.GetComponent<InputField>().text = PlayerPrefs.GetString("name", "New Player");
		Time.timeScale = 1;
    }

	public void updateName() {
        string name = this.GetComponent<InputField>().text;
        if(!name.Equals("")) {
            PlayerPrefs.SetString("name", name);
        }
    }
}
