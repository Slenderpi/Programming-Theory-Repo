using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtOnTouch : MonoBehaviour {

	PlayerManager pm;

	bool hasTouched = false;

	private void Start() {
		pm = GameManager.instance.GetPlayerManager();
	}

	private void OnTriggerEnter(Collider other) {
		if (!hasTouched && other.CompareTag("Player")) {
			hasTouched = true;
			pm.IncreaseHealth(-1);
		}
	}

}
