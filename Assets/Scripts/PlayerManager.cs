//#define DEBUG_DAMAGE_VELOCITY_REDUCTION

using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour {

	Animator anim;

	float horizontalInput = 0;

	float startingHorizontalSpeed = 12f;
	[SerializeField]
	float horizontalMoveSpeed;
	float startingForwardSpeed = 5f;
	[SerializeField]
	public float forwardVel = 0;

	float startingAcceleration = 0.35f;
	float acceleration; // Leave starting at 0
	float horizontalAccelerationProportion = 0.35f; // Proportion of forward acceleration the horizontal acceleration will have
	float timeUntilAccelerationStart = 10f;
	float hurtSpeedReduction = 0.5f; // FONKY MATH please leave at 0.5f :))) ~~A value of 0.6f means forwardVelocity is set to forwardVelocity * 0.6f after getting hurt~~

	float maxInvincibilityTime = 10;
	Coroutine invincibilityCoroutine;
	bool isInvincible = false;

	float timeElapsed = -1;
	float timeElapsedV = 0; // Exists solely for player hurt velocity calculations. Read multiline comment within IncreaseHealth() for more info
	float distanceTraveled = 0;

	bool isPaused = false;

	public TextMeshProUGUI healthText;
	public TextMeshProUGUI pointsText;
	public TextMeshProUGUI timeElapsedText;
	public TextMeshProUGUI distanceTraveledText;
	public GameObject pausePanel;
	public GameObject gameoverPanel;
	public TextMeshProUGUI gameoverGemsText;
	public TextMeshProUGUI gameoverDistanceText;
	public TextMeshProUGUI gameoverTimeText;

	public GameObject[] invincibilityIndicators;

	public ParticleSystem obtainHealthParticleExplosion;
	public ParticleSystem obtainInvincibilityParticleExplosion;
	public ParticleSystem obtainGemParticleExplosion;

	public int health = 3;
	public int gemsCollected { get; private set; }

	public float horizontalVelocity { get; private set; }
	public float forwardVelocity { get; private set; }

	// Start is called before the first frame update
	void Start() {
		anim = GetComponent<Animator>();
		gemsCollected = 0;

		healthText.gameObject.SetActive(false);
		pointsText.gameObject.SetActive(false);
		timeElapsedText.gameObject.SetActive(false);
		distanceTraveledText.gameObject.SetActive(false);
		pausePanel.SetActive(false);
		gameoverPanel.SetActive(false);

		healthText.text = $"Health: {health}";

		anim.SetFloat("Speed_f", 0.26f);
	}

	// Update is called once per frame
	void Update() {
		horizontalInput = Input.GetAxis("Horizontal");

		forwardVelocity += acceleration * Time.deltaTime;

		forwardVel = forwardVelocity; // For debug reasons

		horizontalVelocity = horizontalMoveSpeed * horizontalInput;
		horizontalMoveSpeed += acceleration * horizontalAccelerationProportion * Time.deltaTime;
		//horizontalMoveSpeed = forwardVelocity * (startingHorizontalSpeed / startingForwardSpeed);

		//groundMaterial.SetTextureOffset()

		anim.speed = (forwardVelocity - horizontalMoveSpeed) / 100f + 1;

		if (timeElapsed != -1 && health > 0) {
			timeElapsed += Time.deltaTime;
			timeElapsedV += Time.deltaTime;
			distanceTraveled += forwardVelocity * Time.deltaTime;
			timeElapsedText.text = TimeSpan.FromSeconds(timeElapsed).ToString("m':'ss'.'f");
			distanceTraveledText.text = String.Format("{0:F1} m", Mathf.Round(distanceTraveled * 10) / 10f);
		}

		if (Input.GetKeyDown(KeyCode.Escape) && !GameManager.instance.isGameOver) {
			isPaused = !isPaused;
			pausePanel.SetActive(isPaused);
			Time.timeScale = isPaused ? 0 : 1;
		}

		//if (Input.GetKeyDown(KeyCode.R)) {
		//	IncreaseHealth(1);
		//} else if (Input.GetKeyDown(KeyCode.Space)) {
		//	IncreaseHealth(-1);
		//}
	}

	public void StartMoving() {
		print("Start acceleration routine");
		StartCoroutine(StartAccelerating());
		print("Start camera routine");
		StartCoroutine(PanCameraBackward());
	}

	IEnumerator StartAccelerating() {
		forwardVelocity = startingForwardSpeed;
		horizontalMoveSpeed = startingHorizontalSpeed;
		yield return new WaitForSeconds(3);
		timeElapsed = 0;
		healthText.gameObject.SetActive(true);
		pointsText.gameObject.SetActive(true);
		timeElapsedText.gameObject.SetActive(true);
		distanceTraveledText.gameObject.SetActive(true);
		yield return new WaitForSeconds(timeUntilAccelerationStart);
		acceleration = startingAcceleration;
		anim.SetFloat("Speed_f", 0.6f);
	}

	IEnumerator StartInvincibilityTimer() {
		isInvincible = true;
		obtainInvincibilityParticleExplosion.Play();
		foreach (GameObject indicator in invincibilityIndicators) {
			indicator.SetActive(true);
		}
		yield return new WaitForSeconds(maxInvincibilityTime);
		isInvincible = false;
		foreach (GameObject indicator in invincibilityIndicators) {
			indicator.SetActive(false);
		}
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
			else {
				if (amnt < 0) {
#if DEBUG_DAMAGE_VELOCITY_REDUCTION
					// IDK HOW TO TO DO MATH. Speed reduction calculations will be assuming sppedReduction is 0.5f
					//forwardVelocity *= hurtSpeedReduction; // <-- this is what should be happening. But since I can't do math, we won't be doing this
					/*
					 * tried to figure this out on Desmos. Did not figure it out
					 * fx is the function for forwardVelocity. It is equal to startingVelocity + acceleration * (TIME - timeUntilAccelStart)
					 * TIME refers to any point in time. There will be two used: the "current" time notated as 'v' and a point of time in the past, 'u'.
					 *		v will not be the true "current" time because as the player gets hurt more and more the true current time does not represent
					 *		the time that the velocity equations will be existing in. Everytime the player is hurt the velocities are sent back in time.
					 *		Because of this, the variable timeElapsedV exists explicitly for these calcualtions. So, v = timeElapsedV
					 * Assuming a speedReduction of 0.5, u is equal to v * 0.5 + 3. It just is.
					 * Using fx and these variables, we can calculate the forwardVelocity at time u in the past, because u represents the
					 *		time fx was at half the velocity it is now (at time v)
					 * The new forwardVelocity will thus be fxU (variable name just means inputting U into fx, thus 'fxU')
					 * 
					 * Because we know what time fx was at half its velocity using u, we can also find what the horizontal velocity, hx,
					 *		would be at time u. Essentially, u helps us rewind time for both forwardVelocity and horizontalMoveSpeed.
					 * hxU would simply be hxU = startingHorizontal + acceleration * horizontalProportion * (u - timeUntilAccel)
					 */
					float v = timeElapsedV; // v is the current point in time
					float u = v * hurtSpeedReduction + 3; // u is the point in time in the past where fxU is half the velocity of fxV
					float fxU = startingForwardSpeed + acceleration * (u - timeUntilAccelerationStart);
					float hxU = startingHorizontalSpeed + acceleration * horizontalAccelerationProportion * (u - timeUntilAccelerationStart);
					timeElapsedV = u; // Set timeElapsedV to the appropriate rewound time, which is simply u
					print($"Initial values:\nForward: {forwardVelocity}\nHorizontal: {horizontalMoveSpeed}\nCalculations:\nv: {v}\nu: {u}\nfxU: {fxU}\nfxV: {hxU}");
					forwardVelocity = fxU;
					horizontalMoveSpeed = hxU;
#else
					float u = timeElapsedV * hurtSpeedReduction + 3;
					forwardVelocity = startingForwardSpeed + acceleration * (u - timeUntilAccelerationStart);
					horizontalMoveSpeed = startingHorizontalSpeed + acceleration * horizontalAccelerationProportion * (u - timeUntilAccelerationStart);
					timeElapsedV = u;
#endif
					if (forwardVelocity < startingForwardSpeed) {
						forwardVelocity = startingForwardSpeed;
					}
					if (horizontalMoveSpeed < startingHorizontalSpeed) {
						horizontalMoveSpeed = startingHorizontalSpeed;
					}
				} else {
					obtainHealthParticleExplosion.Play();
				}
				health += amnt;
			}

			healthText.text = $"Health: {health}";

			if (health <= 0) {
				GameManager.instance.GameOver();

				Time.timeScale = 1;
				isPaused = false;
				pausePanel.SetActive(false);

				acceleration = 0;
				forwardVelocity = 0;
				horizontalMoveSpeed = 0;

				StartCoroutine(PanCameraBackward());
				anim.speed = 1;
				anim.SetInteger("DeathType_int", 1);
				anim.SetBool("Death_b", true);

				StartCoroutine(ShowGameoverScreen());
			}
		}
	}

	public void IncreaseGemCount(int amnt) {
		gemsCollected += amnt;
		pointsText.text = $"Gems: {gemsCollected}";
		obtainGemParticleExplosion.Play();
	}

	IEnumerator PanCameraBackward() {
		Rigidbody cameraRb = Camera.main.gameObject.GetComponent<Rigidbody>();
		cameraRb.velocity = new Vector3(0, 0, -1.2f);
		yield return new WaitForSeconds(3);
		cameraRb.velocity = new Vector3(0, 0, 0);
	}

	IEnumerator ShowGameoverScreen() {
		yield return new WaitForSeconds(1.5f);
		gameoverPanel.SetActive(true);
		healthText.gameObject.SetActive(false);
		pointsText.gameObject.SetActive(false);
		timeElapsedText.gameObject.SetActive(false);
		distanceTraveledText.gameObject.SetActive(false);

		gameoverGemsText.text = "" + gemsCollected;
		gameoverDistanceText.text = String.Format("{0:F1} m", Mathf.Round(distanceTraveled * 10) / 10f);
		gameoverTimeText.text = TimeSpan.FromSeconds(timeElapsed).ToString("m':'ss'.'f");
	}

	public void RestartGame() {
		GameManager.instance.StartGame();
	}

}
