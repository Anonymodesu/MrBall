using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//detects when the ball falls out of bounds (dies)
//attached to the Player gameObject
public class Script_OutOfBounds : MonoBehaviour {

	private Vector3 maxima; //max x,y,z values
	private Vector3 minima; //min x,y,z values
	private const float leeway = 15;

	// Use this for initialization
	void Start () {
		setExtrema();
	}
	

	public bool outOfBounds() {
		Vector3 pos = gameObject.transform.position;

		//if either of these conditions are true, then the ball has exited the bounding box
		return maxima != Vector3.Max(maxima, pos) || minima != Vector3.Min(minima, pos);
	}
	
	private void setExtrema() { //find a bounding box for the playing field
		maxima = Vector3.negativeInfinity;
		minima = Vector3.positiveInfinity;
	
		GameObject playingField = GameObject.Find("Rollercoaster");
		setExtrema(playingField.transform, ref maxima, ref minima);
		
		GameObject cubies = GameObject.Find("Cubies");
		foreach(Transform child in cubies.transform) {
			maxima = Vector3.Max(maxima, child.position);
			minima = Vector3.Min(minima, child.position);
		}
		
		maxima = maxima + Vector3.one * leeway;
		minima = minima - Vector3.one * leeway;
	}
	
	//recursive helper method for setting the bounding box of the playing field
	private void setExtrema(Transform parent, ref Vector3 maxima, ref Vector3 minima) {
		
		foreach(Transform child in parent) {
			Collider collider = child.gameObject.GetComponent<Collider>();
		
			if(collider == null) { //child is an organisational container for other objects
				setExtrema(child, ref maxima, ref minima);
				
			} else { //the child is a physical object; these objects are assumed to have no children
				maxima = Vector3.Max(maxima, collider.bounds.max); //Max() returns the maximal x,y,z components of the two vectors
				minima = Vector3.Min(minima, collider.bounds.min);
			}
		}
		
	}
}
