using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	PlayerManager pm;
	SpawnManager sm;

	public bool isGameOver { get; private set; }
	public bool isGameStarted { get; private set; }

	private void Awake() {
		if (instance) {
			Destroy(gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	// Start is called before the first frame update
	void Start() {
		isGameOver = false;
		isGameStarted = false;
		//pm = GameObject.Find("Player").GetComponent<PlayerManager>();
		//sm = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
		if (SceneManager.GetActiveScene().name == "IngameScene") {
			StartGame(); // Force start
		}
	}

	// Update is called once per frame
	void Update() {

	}

	public void StartGame() {
		// TODO
		isGameOver = false;
		SceneManager.LoadScene("IngameScene");
		StartCoroutine(SetManagersAndStartGame());
	}

	IEnumerator SetManagersAndStartGame() {
		GameObject player, spawner;
		do {
			player = GameObject.Find("Player");
			spawner = GameObject.Find("SpawnManager");
			yield return new WaitForEndOfFrame();
		} while (player == null || spawner == null);
		pm = player.GetComponent<PlayerManager>();
		sm = spawner.GetComponent<SpawnManager>();

		isGameStarted = true;

		yield return new WaitForSeconds(1);
		pm.StartMoving();
		sm.StartSpawning();
	}

	public void GameOver() {
		isGameOver = true;
		print("Game over!");
		sm.OnGameOver();
	}

	public PlayerManager GetPlayerManager() {
		return pm;
	}

}
