using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Cubies : MonoBehaviour {

	private const float rotationSpeed = 1f;
	private const float rotationChangeSpeed = 0.01f;
	
	private Vector3 axis;

	void Start() {
		axis = Random.insideUnitSphere.normalized * rotationSpeed;
	}

	// Update is called once per frame
	void Update () {
		transform.Rotate(axis, rotationSpeed * Time.deltaTime * 60, Space.World);
		axis = (axis + Random.insideUnitSphere.normalized * rotationChangeSpeed * Time.deltaTime * 60).normalized;
	}

}
