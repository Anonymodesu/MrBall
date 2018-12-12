using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StairsInstantiate : ScriptableWizard {

	public GameObject target;
	public int count = 1;
	public float length = 1;
	public float width = 1;
	public float height = 0.1f;

	[MenuItem ("My Tools/Stairs Instantiate")]
	static void StairsInstantiateWizard() {
		ScriptableWizard.DisplayWizard<StairsInstantiate>("Generate stairs", "Generate");
	}

	void OnWizardCreate() {
		if(target == null) {
			Debug.Log("must specify an object!");
			return;
		}

		GameObject parent = new GameObject("Stairs");

		for(int i = 0; i < count; i++) {
			Transform step = ((GameObject) PrefabUtility.InstantiatePrefab(target)).transform;
			step.parent = parent.transform;
			step.position = new Vector3(i * length, i * height, 0);
			step.localScale = new Vector3(length, height, width);
		}
	}
}
