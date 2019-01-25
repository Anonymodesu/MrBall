using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Menu_Settings_Button : Button {

	public GameObject target;

	//disables all siblings of target
	public void switchSettingsMenu() {
		foreach(Transform child in target.transform.parent) {
			child.gameObject.SetActive(false);
		}

		target.SetActive(true);
	}
}
