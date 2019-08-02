using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RadialInstantiate : ScriptableWizard {
	[System.NonSerialized]
	private bool loadedValues = false;

	[System.NonSerialized]
	public GameObject target;
	
	public float radius = 0;
	public float step = 0;
	public float arcDegrees = 360;
	public int number = 0;

	[MenuItem ("My Tools/Radial Instantiate")]
	static void RadialInstantiateWizard() {
		ScriptableWizard.DisplayWizard<RadialInstantiate>("Radially instantiate objects", "Instantiate");
	}

	void OnWizardUpdate() {
		
		//allows the wizard to remember previously entered values
		if(!loadedValues) {
			string data = EditorPrefs.GetString("RadialInstantiateSettings", JsonUtility.ToJson(this));
			JsonUtility.FromJsonOverwrite(data, this);
			loadedValues = true;
		}

	}

	void OnWizardCreate() {

		if(target == null) {
			Debug.Log("must specify an object!");
			return;
		}

		float angleDelta = arcDegrees / number;
		GameObject parent = new GameObject("parent");

		for(int i = 0; i < number; i++) {
			Transform obj = ((GameObject) PrefabUtility.InstantiatePrefab(target)).transform;

			float angle = Mathf.Deg2Rad * i * angleDelta;
			Vector3 position = radius * (new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
			Quaternion rotation = Quaternion.LookRotation(-position);

			obj.position = position +  i * step * Vector3.up;
			obj.rotation = rotation;
			obj.parent = parent.transform;
		}

		string data = JsonUtility.ToJson(this);
		EditorPrefs.SetString("RadialInstantiateSettings", data);
	}
}
