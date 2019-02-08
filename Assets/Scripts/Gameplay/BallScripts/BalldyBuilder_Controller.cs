using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalldyBuilder_Controller : Player_Controller {

	private List<int> checkpointParents;//used to identify a newly created checkpoint in a quicksave

	private const float transformDuration = 1;

	private GameObject checkpointCreationEffect;
	private GameObject CheckpointCreationEffect {
		get {
			if(checkpointCreationEffect == null) {
				checkpointCreationEffect = GameObject.Find("Resources").GetComponent<Balls>().BalldyBuilderEffect;
			}
			return checkpointCreationEffect;
		}
	}
	private const float effectMultiplier = 100;

	public BalldyBuilder_Controller(Script_Player_Loader playerScript, Player_Jump jumpScript, Player_Move movementScript, Player_Trails trailsScript, 
							GameObject startPos, SphereCollider triggerCollider, List<Script_Ramp_Animator> animatedRamps)
							: base(playerScript, jumpScript, movementScript, trailsScript, 
							startPos, triggerCollider, animatedRamps) {

		if(SettingsManager.QuickSaveLoaded) {
			loadQuickSave();
		} else {
			checkpointParents = null;
		}
	}

	// Update is called once per frame
	public override void Update () {
		base.Update();

		//checkpointParents == null means that a checkpoint has not been created yet
		if(Time.timeScale != 0 && checkpointParents == null && InputManager.getInput().buttonDown(Command.Special)) {
			constructCheckpoint();
		}
	}

	private void constructCheckpoint() {
		//find a green ramp to turn into a checkpoint
		foreach(GameObject ramp in contacts) {

			if(ramp.tag == "Ground") {
				playerScript.StartCoroutine(transformRamp(ramp));
				checkpointParents = getUniqueID(ramp);
				setCheckpoint(ramp, Physics.gravity.normalized);
				break;
			}
		}
	}

	//aesthetically turns a green ramp into a checkpoint
	private IEnumerator transformRamp(GameObject ramp) {
		if(ramp.tag != "Ground") {
			Debug.Log("can't transform " + ramp.name);
			yield break;
		}
		ramp.tag = "Checkpoint";

		//creation special fx
		var shape = CheckpointCreationEffect.GetComponent<ParticleSystem>().shape;
		shape.scale = ramp.transform.lossyScale;
		var emission = CheckpointCreationEffect.GetComponent<ParticleSystem>().emission;
		//rate of emission is how 'flat' the ramp is compared to the current Y-axis
		emission.rateOverTimeMultiplier = effectMultiplier * Vector3.Cross(ramp.transform.lossyScale, Physics.gravity.normalized).magnitude;
		UnityEngine.Object.Instantiate(checkpointCreationEffect, ramp.transform.position, ramp.transform.rotation);

		//color fade
		float time = 0;
		Material mat = ramp.GetComponent<Renderer>().material;
		Color original = mat.color;
		Color target = Color.white;
		while(time < transformDuration) {
			mat.color = Color.Lerp(original, target, time / transformDuration);
			time += Time.deltaTime;
			yield return null;
		}
		mat.color = target;
	}

	public override Quicksave storeQuickSave() {
		Quicksave_BalldyBuilder save = new Quicksave_BalldyBuilder(base.storeQuickSave(), checkpointParents);
		PlayerManager.getInstance().storeQuicksave(player, save);
		return save;
	}

	private void loadQuickSave() {
		Quicksave_BalldyBuilder save = (Quicksave_BalldyBuilder) PlayerManager.getInstance().getQuicksave(player);
		checkpointParents = save.checkpointParents;

		//checkpointParents lists the child indices to reach the saved checkpoint in reversed order
		if(checkpointParents != null) {	
			GameObject checkpoint = findObject(checkpointParents);
			playerScript.StartCoroutine(transformRamp(checkpoint));
		}
	}
}
