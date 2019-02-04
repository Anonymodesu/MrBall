using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsainBowl_Move : Player_Move {

	private const float duration = 4;

	private GameObject superSaiyanEffect;
	private bool superSaiyan; //whether Usain Bowl is currently a Super Saiyan
	public bool superSaiyanable; //whether Usain Bowl can turn Super Saiyan; resets on death

	protected override float normalSpeed {
		get { 
			if(superSaiyan) {
				return Player_Move.superSpeed;
			} else {
				return base.normalSpeed;
			}
		}
	}

	protected override float maxNormalSpeed {
		get { 
			if(superSaiyan) {
				return Player_Move.maxSuperSpeed;
			} else {
				return base.maxNormalSpeed;
			}
		}
	}



	public UsainBowl_Move(Script_Player_Loader playerScript) : base(playerScript) {
		superSaiyan = false;
		superSaiyanable = true;
		superSaiyanEffect = GameObject.Find("Resources").GetComponent<Balls>().UsainBowlEffect;
	}


	public override void processNextInstruction() {
		base.processNextInstruction();

		if(superSaiyanable && Time.timeScale != 0 && InputManager.getInput().buttonDown(Command.Special)) {
			superSaiyanable = false;
			playerScript.StartCoroutine(goSuperSaiyan());
		}
	}

	private IEnumerator goSuperSaiyan() {
		float time = 0;
		superSaiyan = true;
		SoundManager.getInstance().playSoundFX(SoundFX.UsainBowl);
		GameObject ssjEffect = UnityEngine.Object.Instantiate(superSaiyanEffect, playerScript.transform.position, getEffectRotation());

		while(time < duration) {
			ssjEffect.transform.position = playerScript.transform.position;
			ssjEffect.transform.rotation = getEffectRotation();

			time += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}

		superSaiyan = false;
	}

	private Quaternion getEffectRotation() {
		return Quaternion.FromToRotation(Player_Controller.defaultGravityDirection, Physics.gravity);		
	}
}
