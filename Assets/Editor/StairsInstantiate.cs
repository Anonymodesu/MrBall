using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class StairsInstantiate : ScriptableWizard {

	[System.NonSerialized]
	private bool loadedValues = false;

	[System.NonSerialized]
	public GameObject target;

	public int count = 1;
	public float length = 1;
	public float width = 1;
	public float height = 0.1f;
	public float rotation = 0;

	[MenuItem ("My Tools/Stairs Instantiate")]
	static void StairsInstantiateWizard() {
		ScriptableWizard.DisplayWizard<StairsInstantiate>("Generate stairs", "Generate");
	}

	void OnWizardUpdate() {

		//allows the wizard to remember previously entered values
		if(!loadedValues) {
			string data = EditorPrefs.GetString("StairsInstantiateSettings", JsonUtility.ToJson(this));
			JsonUtility.FromJsonOverwrite(data, this);
			loadedValues = true;
		}

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
			step.rotation = Quaternion.Euler(i * rotation, 0, 0);
		}

		string data = JsonUtility.ToJson(this);
		EditorPrefs.SetString("StairsInstantiateSettings", data);
	}
}
