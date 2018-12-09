using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TransformCopier : ScriptableWizard {

	public List<GameObject> source;
	public List<GameObject> target;

	[MenuItem ("My Tools/Copy Transform")]
	static void TransformCopierWizard() {
		ScriptableWizard.DisplayWizard<TransformCopier>("Copy a transform", "Perform copy");
	}

	void OnWizardCreate() {
		if(source.Count != target.Count) {
			Debug.Log("list sizes dont match!");
			return;
		}

		

		for(int i = 0; i < source.Count; i++) {
			if(target[i] == null || source[i] == null) {
				Debug.Log("item " + i + " is null");
				continue;
			}

			Undo.RecordObject(target[i], "Copying transform " + source[i].name);

			target[i].transform.position = source[i].transform.position;
			target[i].transform.rotation = source[i].transform.rotation;
			target[i].transform.localScale = source[i].transform.localScale;

			PrefabUtility.RecordPrefabInstancePropertyModifications(target[i]);
		}
		
	}
}
