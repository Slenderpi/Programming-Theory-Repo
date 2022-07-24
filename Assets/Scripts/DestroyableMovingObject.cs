using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableMovingObject : MonoBehaviour {

	PlayerManager pm;

	int destroyZ = -11;

	public float extraMoveSpeed = 0;

	// Start is called before the first frame update
	protected virtual void Start() {
		pm = GameManager.instance.GetPlayerManager();
	}

	// Update is called once per frame
	protected virtual void Update() {
		Move();

		if (transform.position.z <= destroyZ)
			Destroy(gameObject);
	}

	protected virtual void Move() {
		transform.position -= new Vector3(pm.horizontalVelocity * Time.deltaTime, 0, (pm.forwardVelocity + extraMoveSpeed) * Time.deltaTime);
	}

}
