using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
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
	}
}
#endif