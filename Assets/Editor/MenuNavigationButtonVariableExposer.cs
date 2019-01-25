using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

//when extending the button class, public/serialized fields aren't exposed in the inspector for some reason
//so we create an artificial field here
[CustomEditor( typeof( Script_Menu_Navigation_Button ) )]
public class MenuNavigationButtonVariableExposer : ButtonEditor {

	public override void OnInspectorGUI()
     {
         Script_Menu_Navigation_Button targetMenuButton = (Script_Menu_Navigation_Button) target;
 	
 		 base.OnInspectorGUI();    // Show default inspector property editor

 		 Undo.RecordObject(targetMenuButton, "Loaded destination into navigation button.");

         targetMenuButton.destination = (GameObject)
         	EditorGUILayout.ObjectField("Destination",  targetMenuButton.destination, typeof(GameObject), true);

         PrefabUtility.RecordPrefabInstancePropertyModifications(targetMenuButton);
 
     }
}
