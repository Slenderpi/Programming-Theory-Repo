using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	PlayerManager pm;
	SpawnManager sm;

	//public AudioSource asource { get; private set; }
	AudioSource asource;

	public bool isGameOver { get; private set; }
	public bool isGameStarted { get; private set; }

	public AudioClip startSound;
	public AudioClip gameMusic;

	public GameObject ingameMusicPlayerObject;

	private void Awake() {
		if (instance) {
			Destroy(gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			DontDestroyOnLoad(ingameMusicPlayerObject);
		}
	}

	// Start is called before the first frame update
	void Start() {
		isGameOver = false;
		isGameStarted = false;
		asource = GetComponent<AudioSource>();

		AudioSource ingameASource = ingameMusicPlayerObject.GetComponent<AudioSource>();
		ingameASource.loop = true;
		ingameASource.clip = gameMusic;
		ingameASource.volume = asource.volume * 0.6f;
		ingameASource.Play();

		if (SceneManager.GetActiveScene().name == "IngameScene") {
			StartGame(); // Force start
		}
		SceneManager.sceneLoaded += SetManagersAndStartGame;
	}

	// Update is called once per frame
	void Update() {

	}

	public void StartGame() {
		// TODO
		asource.PlayOneShot(startSound, 1.4f);
		isGameOver = false;
		SceneManager.LoadScene("IngameScene", LoadSceneMode.Single);
	}

	void SetManagersAndStartGame(Scene scene, LoadSceneMode mode) {
		GameObject player = null;
		GameObject spawner = null;
		do {
			GameObject p = GameObject.Find("Player");
			if (player != p) player = p;
			else print("INCORRECT PLAYER FOUND");
			GameObject s = GameObject.Find("SpawnManager");
			if (spawner != s) spawner = s;
		} while (player == null || spawner == null);
		pm = player.GetComponent<PlayerManager>();
		sm = spawner.GetComponent<SpawnManager>();

		isGameStarted = true;

		pm.StartMoving();
		sm.StartSpawning();
	}

	public void GameOver() {
		isGameOver = true;
		sm.OnGameOver();
	}

	public PlayerManager GetPlayerManager() {
		return pm;
	}

	public AudioSource GetAudioSource() {
		return asource;
	}

}
