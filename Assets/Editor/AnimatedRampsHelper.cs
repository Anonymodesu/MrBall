using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Script_Player))]
public class AnimatedRampsQuicksaveHelper : Editor {


	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		Script_Player targetScript = (Script_Player) target;


		if(GUILayout.Button("Load Ramps Blyat")) {

			Undo.RecordObject(targetScript, "Loaded new set of animated ramps for quicksave.");

			targetScript.animatedRamps.Clear();
			targetScript.animatedRamps.AddRange( (Script_Ramp_Animator[]) FindObjectsOfType<Script_Ramp_Animator>() );

			PrefabUtility.RecordPrefabInstancePropertyModifications(targetScript);
		}

		if(GUILayout.Button("Show Ramps Blyat")) {
			GameObject[] objects = new GameObject[targetScript.animatedRamps.Count]; 

			for(int i = 0; i < targetScript.animatedRamps.Count; i++) {
				objects[i] = targetScript.animatedRamps[i].gameObject;
			}

			Selection.objects = objects;
		}
	}
}
