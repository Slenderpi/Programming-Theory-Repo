using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour {

	bool hasTouched = false;

	protected PlayerManager pm;

	// Start is called before the first frame update
	void Start() {
		pm = GameManager.instance.GetPlayerManager();
	}

	// Update is called once per frame
	void Update() {

	}

	private void OnTriggerEnter(Collider other) {
		if (!hasTouched && other.CompareTag("Player")) {
			hasTouched = true;
			OnCollectPowerup();
			Destroy(gameObject);
		}
	}

	protected abstract void OnCollectPowerup();

}
