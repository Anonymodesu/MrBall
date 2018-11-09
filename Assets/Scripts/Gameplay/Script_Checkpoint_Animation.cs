using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Script_Checkpoint_Animation : MonoBehaviour {

	private const float animationTime = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//makes the checkpoint animation fade in and then fade out
	public IEnumerator animate() {
		Text text = GetComponentInChildren<Text>();
		Image image = GetComponentInChildren<Image>();

		Color originalTextColor = text.color;
		Color originalImageColor = image.color;
		originalTextColor.a = 0;
		originalImageColor.a = 0;

		Color targetTextColor = text.color;
		Color targetImageColor = image.color;
		targetTextColor.a = 1;
		targetImageColor.a = 1;

		for(float i = 0; i < animationTime; i += Time.deltaTime) {
			text.color = Color.Lerp(originalTextColor, targetTextColor, i / animationTime);
			image.color = Color.Lerp(originalImageColor, targetImageColor, i / animationTime);
			yield return null;
		}
		text.color = targetTextColor;
		image.color = targetImageColor;

		yield return new WaitForSeconds(animationTime);

		for(float i = 0; i < animationTime; i += Time.deltaTime) {
			text.color = Color.Lerp(targetTextColor, originalTextColor, i / animationTime);
			image.color = Color.Lerp(targetImageColor, originalImageColor, i / animationTime);
			yield return null;
		}
		text.color = originalTextColor;
		image.color = originalImageColor;
	}
}
