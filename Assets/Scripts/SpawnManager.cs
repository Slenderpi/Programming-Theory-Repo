using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

	public GameObject[] animals;

	public GameObject[] powerups;

	PlayerManager pm;

	float startingAnimalSpawnDelay = 3.5f;
	float animalSpawnDelay;
	float minAnimalSpawnDelay = 0.3f;
	float animalSpawnDelayOffset = 0.2f;

	float powerupSpawnDelay = 1f;
	float powerupSpawnDelayOffset = 0.2f;
	float chancePowerupIsGem = 0.9f;

	int difficultyLevel = 1;
	float difficultyIncreaseDelay = 8;

	float spawnRangeX = 30f;
	float spawnZ;

	Coroutine spawnAnimalCoroutine;
	Coroutine spawnPowerupCoroutine;
	Coroutine increaseDifficultyCoroutine;

	// For debugging purposes
	public GameObject spawnVisualizerLeft;
	public GameObject spawnVisualizerRight;
	public bool maxedDifficulty = false;

	// Start is called before the first frame update
	void Start() {
		spawnZ = transform.position.z;
		animalSpawnDelay = startingAnimalSpawnDelay;
	}

	public void StartSpawning() {
		pm = GameManager.instance.GetPlayerManager();

		spawnAnimalCoroutine = StartCoroutine(SpawnAnimals());
		spawnPowerupCoroutine = StartCoroutine(SpawnPowerups());
		increaseDifficultyCoroutine = StartCoroutine(IncreaseDifficulty());
	}

	// Update is called once per frame
	//void Update() {
	//	spawnVisualizerLeft.transform.position = new Vector3(-spawnRangeX + GameManager.instance.GetPlayerManager().horizontalVelocity, 0, spawnZ);
	//	spawnVisualizerRight.transform.position = new Vector3(spawnRangeX + GameManager.instance.GetPlayerManager().horizontalVelocity, 0, spawnZ);
	//}

	IEnumerator SpawnAnimals() {
		yield return new WaitForSeconds(3);
		float currSpawnDelay = 0;
		while (true) {
			yield return new WaitForSeconds(currSpawnDelay);
			GameObject animalToSpawn = animals[Random.Range(0, difficultyLevel)];
			Instantiate(animalToSpawn, new Vector3(Random.Range(-spawnRangeX, spawnRangeX) + pm.horizontalVelocity, 0.05f, spawnZ), animalToSpawn.transform.rotation);
			currSpawnDelay = Random.Range(animalSpawnDelay - animalSpawnDelayOffset, animalSpawnDelay + animalSpawnDelayOffset);
		}
	}

	IEnumerator SpawnPowerups() {
		yield return new WaitForSeconds(3);
		float currSpawnDelay = powerupSpawnDelay;
		while (true) {
			yield return new WaitForSeconds(currSpawnDelay);
			GameObject toSpawn = powerups[Random.Range(0f, 1f) <= chancePowerupIsGem ? 0 : Random.Range(1, powerups.Length)];
			if (toSpawn.name == "InvincibilityPower") {
				toSpawn.GetComponent<ParticleSystem>();
			}
			Instantiate(toSpawn, new Vector3(Random.Range(-spawnRangeX, spawnRangeX) + pm.horizontalVelocity, 1, spawnZ), toSpawn.transform.rotation);
			currSpawnDelay = Random.Range(powerupSpawnDelay - powerupSpawnDelayOffset, powerupSpawnDelay + powerupSpawnDelayOffset);
		}
	}

	IEnumerator IncreaseDifficulty() {
		yield return new WaitForSeconds(3);
		for (int i = 0; i < animals.Length - 1; i++) {
			yield return new WaitForSeconds(difficultyIncreaseDelay);
			animalSpawnDelay = startingAnimalSpawnDelay - difficultyLevel++ / (animals.Length - 1) * (startingAnimalSpawnDelay - minAnimalSpawnDelay);
		}
		increaseDifficultyCoroutine = null;
		maxedDifficulty = true;
	}

	public void OnGameOver() {
		StopCoroutine(spawnAnimalCoroutine);
		StopCoroutine(spawnPowerupCoroutine);
		if (increaseDifficultyCoroutine != null)
			StopCoroutine(increaseDifficultyCoroutine);
	}

}
