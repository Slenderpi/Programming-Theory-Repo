using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour {

	Rigidbody rb;

	[SerializeField]
	float speed = 50;
	[SerializeField]
	float rotationSpeed = 150;

	float verticalInput;
	float horizontalInput;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate() {
		verticalInput = Input.GetAxis("Vertical");
		horizontalInput = Input.GetAxis("Horizontal");
		rb.velocity = (transform.forward * verticalInput) * speed * Time.fixedDeltaTime;
		transform.Rotate((transform.up * horizontalInput) * rotationSpeed * Time.fixedDeltaTime);
	}

}
