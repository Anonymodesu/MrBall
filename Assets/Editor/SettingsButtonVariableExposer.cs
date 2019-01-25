using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

//when extending the button class, public/serialized fields aren't exposed in the inspector for some reason
//so we create an artificial field here
[CustomEditor( typeof( Script_Menu_Settings_Button ) )]
public class SettingsButtonVariableExposer : ButtonEditor {

	public override void OnInspectorGUI()
     {
         Script_Menu_Settings_Button targetButton = (Script_Menu_Settings_Button) target;
 	
 		 base.OnInspectorGUI();    // Show default inspector property editor

 		 Undo.RecordObject(targetButton, "Loaded destination into navigation button.");

         targetButton.target = (GameObject)
         	EditorGUILayout.ObjectField("Target menu",  targetButton.target, typeof(GameObject), true);

         PrefabUtility.RecordPrefabInstancePropertyModifications(targetButton);
 
     }
}
