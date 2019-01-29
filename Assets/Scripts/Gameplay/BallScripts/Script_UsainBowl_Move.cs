using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_UsainBowl_Move : Script_Player_Move {

	private const float duration = 4;

	private GameObject superSaiyanEffect;

	private bool superSaiyan;

	protected override float normalSpeed {
		get { 
			if(superSaiyan) {
				return Script_Player_Move.superSpeed;
			} else {
				return base.normalSpeed;
			}
		}
	}

	protected override float maxNormalSpeed {
		get { 
			if(superSaiyan) {
				return Script_Player_Move.maxSuperSpeed;
			} else {
				return base.maxNormalSpeed;
			}
		}
	}



	protected override void Start() {
		base.Start();
		superSaiyan = false;
		superSaiyanEffect = GameObject.Find("Resources").GetComponent<Balls>().getEffect(BallType.UsainBowl);
	}


	void Update() {
		if(!superSaiyan && Time.timeScale != 0 && InputManager.getInput().buttonDown(Command.Special)) {
			StartCoroutine(goSuperSaiyan());
		}
	}

	private IEnumerator goSuperSaiyan() {
		float time = 0;
		superSaiyan = true;
		SoundManager.getInstance().playSoundFX(SoundFX.UsainBowl);
		GameObject ssjEffect = Instantiate(superSaiyanEffect, transform.position, getEffectRotation());

		while(time < duration) {
			ssjEffect.transform.position = this.transform.position;
			ssjEffect.transform.rotation = getEffectRotation();

			time += Time.deltaTime;
			yield return new WaitForFixedUpdate();
		}

		superSaiyan = false;
	}

	private Quaternion getEffectRotation() {
		return Quaternion.LookRotation(-Physics.gravity);		
	}

}
