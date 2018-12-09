using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Script_Menu_Camera))]
public class MenuNavigator : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		Transform cameraPos = ((Script_Menu_Camera) target).transform;

		if(GUILayout.Button("Level select")) {
			Transform dest = (GameObject.Find("Level Select Menu").transform);
			cameraPos.position = dest.position - 2 * dest.forward + Vector3.up * 0.2f;
			cameraPos.rotation = Quaternion.LookRotation(dest.forward);	
		}

		if(GUILayout.Button("High Scores")) {
			Transform dest = (GameObject.Find("High Scores Menu").transform);
			cameraPos.position = dest.position - 2 * dest.forward + Vector3.up * 0.2f;
			cameraPos.rotation = Quaternion.LookRotation(dest.forward);	
		}

		if(GUILayout.Button("SettingsMenu")) {
			Transform dest = (GameObject.Find("Settings Menu").transform);
			cameraPos.position = dest.position - 2 * dest.forward + Vector3.up * 0.2f;
			cameraPos.rotation = Quaternion.LookRotation(dest.forward);	
		}

		if(GUILayout.Button("Credits")) {
			Transform dest = (GameObject.Find("Credits Menu").transform);
			cameraPos.position = dest.position - 2 * dest.forward + Vector3.up * 0.2f;
			cameraPos.rotation = Quaternion.LookRotation(dest.forward);	
		}
	}
}
#endif