using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour {

	Animator anim;

	float forwardInput = 0;
	float horizontalInput = 0;

	float horizontalMoveSpeed = 10f;
	float startingForwardSpeed = 5f;

	float tempAccel = 0.25f; // Set this value to set acceleration
	float acceleration; // Leave starting at 0
	float horizontalAccelerationProportion = 0.25f; // Proportion of forward acceleration the horizontal acceleration will have
	float timeUntilAccelerationStart = 10f;

	float maxInvincibilityTime = 10;
	Coroutine invincibilityCoroutine;
	bool isInvincible = false;

	float timeElapsed = -1;

	public TextMeshProUGUI healthText;
	public TextMeshProUGUI pointsText;
	public TextMeshProUGUI timeElapsedText;

	public Material groundMaterial;

	public int health = 3;
	public float points { get; private set; }

	public float horizontalVelocity { get; private set; }
	public float forwardVelocity { get; private set; }
	public float currVelocity; // for debug reasons

	// Start is called before the first frame update
	void Start() {
		anim = GetComponent<Animator>();
		points = 0;

		healthText.gameObject.SetActive(false);
		pointsText.gameObject.SetActive(false);
		timeElapsedText.gameObject.SetActive(false);

		healthText.text = $"Health: {health}";

		anim.SetFloat("Speed_f", 0.26f);
	}

	// Update is called once per frame
	void Update() {
		forwardInput = Input.GetAxis("Vertical");
		horizontalInput = Input.GetAxis("Horizontal");

		if (timeElapsed != -1 && health > 0) {
			timeElapsed += Time.deltaTime;
			timeElapsedText.text = TimeSpan.FromSeconds(timeElapsed).ToString("m':'ss'.'f");
		}

		forwardVelocity += acceleration * Time.deltaTime;
		currVelocity = forwardVelocity;

		horizontalVelocity = horizontalMoveSpeed * horizontalInput;
		horizontalMoveSpeed += acceleration * horizontalAccelerationProportion * Time.deltaTime;

		//groundMaterial.SetTextureOffset()

		anim.speed = (forwardVelocity - horizontalMoveSpeed) / 100f + 1;
	}

	public void StartMoving() {
		StartCoroutine(StartAccelerating());
		StartCoroutine(PanCameraBackward());
	}

	IEnumerator StartAccelerating() {
		forwardVelocity = startingForwardSpeed;
		yield return new WaitForSeconds(3);
		timeElapsed = 0;
		healthText.gameObject.SetActive(true);
		pointsText.gameObject.SetActive(true);
		timeElapsedText.gameObject.SetActive(true);
		yield return new WaitForSeconds(timeUntilAccelerationStart);
		acceleration = tempAccel;
		anim.SetFloat("Speed_f", 0.6f);
	}

	IEnumerator StartInvincibilityTimer() {
		isInvincible = true;
		yield return new WaitForSeconds(maxInvincibilityTime);
		isInvincible = false;
	}

	public void EnableInvincibility() {
		if (isInvincible)
			StopCoroutine(invincibilityCoroutine);
		invincibilityCoroutine = StartCoroutine(StartInvincibilityTimer());
	}

	public void IncreaseHealth(int amnt, bool isInstaKill = false) {
		if (health > 0 && (amnt > 0 || !isInvincible)) {
			if (isInstaKill)
				health = 0;
			else
				health += amnt;

			healthText.text = $"Health: {health}";

			if (health <= 0) {
				GameManager.instance.GameOver();
				acceleration = 0;
				forwardVelocity = 0;
				horizontalMoveSpeed = 0;
				StartCoroutine(PanCameraBackward());
				anim.speed = 1;
				anim.SetInteger("DeathType_int", 1);
				anim.SetBool("Death_b", true);
			}
		}
	}

	public void IncreasePoints(int amnt) {
		points += amnt;
		pointsText.text = $"Points: {points}";
	}

	IEnumerator PanCameraBackward() {
		Rigidbody cameraRb = Camera.main.gameObject.GetComponent<Rigidbody>();
		cameraRb.velocity = new Vector3(0, 0, -1.2f);
		yield return new WaitForSeconds(3);
		cameraRb.velocity = new Vector3(0, 0, 0);
	}

}
