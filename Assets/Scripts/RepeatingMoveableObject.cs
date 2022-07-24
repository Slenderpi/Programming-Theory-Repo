using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatingMoveableObject : MonoBehaviour {

	PlayerManager pm;

	[SerializeField]
	float startX;
	[SerializeField]
	float startZ;
	[SerializeField]
	float xLimit;
	[SerializeField]
	float zLimit;

	[SerializeField]
	float xSize;
	[SerializeField]
	float zSize;

	// Start is called before the first frame update
	void Start() {
		startX = transform.position.x;
		startZ = transform.position.z;

		xSize = GetComponent<BoxCollider>().size.x / 10f * transform.localScale.x;
		zSize = GetComponent<BoxCollider>().size.z / 10f * transform.localScale.z;

		xLimit = startX - xSize;
		zLimit = startZ - zSize;
	}

	// Update is called once per frame
	void Update() {
		if (GameManager.instance.isGameStarted) {
			transform.position -= new Vector3(GameManager.instance.GetPlayerManager().horizontalVelocity * Time.deltaTime, 0, GameManager.instance.GetPlayerManager().forwardVelocity * Time.deltaTime);

			if (transform.position.x < xLimit || transform.position.x > -xLimit) {
				transform.position = new Vector3(startX, transform.position.y, transform.position.z);
			}
			if (transform.position.z < zLimit) {
				transform.position = new Vector3(transform.position.x, transform.position.y, startZ);
			}
		}
	}

}
