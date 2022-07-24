using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryObject : DestroyableMovingObject {

	Animator anim;

	bool isPowerup = false;
	
	public float rotateSpeed = 0;

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();
		extraMoveSpeed = 0;
		if (TryGetComponent<Animator>(out anim)) {
			anim.speed = 1.25f;
			anim.SetFloat("Speed_f", 0);
			anim.SetBool("Eat_b", true);

			StartCoroutine(StopEating());
		} else {
			// This GameObject is a powerup
			isPowerup = true;
		}
	}

	protected override void Update() {
		base.Update();

		if (isPowerup) {
			// Spin
			transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
		}
	}

	IEnumerator StopEating() {
		yield return new WaitForSeconds(4);
		anim.SetBool("Eat_b", false);
	}

}
