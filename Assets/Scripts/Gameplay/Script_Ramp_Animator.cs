using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Script_Ramp_Animator : MonoBehaviour {

	#pragma warning disable 0649

	[SerializeField]
	private AnimationClip animationClip;

	[SerializeField]
	private float timeOffset;

	#pragma warning restore 0649

	private AnimatorOverrideController animControl;
	private Animator anim;

	// Sets the animation clip specific to this ramp
	// This was implemented to avoid creating a new animation controller for each animated ramp
	void Awake () {

		anim = GetComponent<Animator>();
		animControl = new AnimatorOverrideController(anim.runtimeAnimatorController);
		animControl["blankAnimation"] = animationClip;
		anim.runtimeAnimatorController = animControl;

		if(SettingsManager.QuickSaveLoaded) {
			//settings are loaded by Script_Player

		} else { //start playing on the normally assigned time
			setNormalisedPlayTime(timeOffset);
		}

	}

	public float getNormalisedPlayTime() {
		return anim.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
	}

	public void setNormalisedPlayTime(float time) {

		anim.Play(0, -1, time);
	}

}
