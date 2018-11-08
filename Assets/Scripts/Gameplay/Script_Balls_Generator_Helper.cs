using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Script_Balls_Generator))]
public class Script_Balls_Generator_Helper : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		Script_Balls_Generator targetScript = (Script_Balls_Generator) target;
		if(GUILayout.Button("Construct Balls Blyat")) {
			targetScript.makeBalls();
		}
	}
}
#endif
