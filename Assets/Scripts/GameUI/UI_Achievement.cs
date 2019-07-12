using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_Achievement : MonoBehaviour
{
	#pragma warning disable 0649
	[SerializeField]
	private Text achievement;
	[SerializeField]
	private Image trophy;
	[SerializeField]
	private Text newNotice;
	#pragma warning restore 0649

    public void loadValues(Achievement.Evaluation eval, bool displayNew = true) {
		achievement.text = String.Format("{0}: {1} / {2}", eval.field, eval.record, eval.requirement);
		trophy.enabled = eval.requirementSatisfied;
		newNotice.enabled = eval.newRequirementSatisfied && displayNew;
    }


    public void reset() {
    	achievement.text = "-";
    	trophy.enabled = false;
		newNotice.enabled = false;
    }
}
