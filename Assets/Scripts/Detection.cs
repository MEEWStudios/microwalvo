using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Detection : MonoBehaviour {
	public GameObject managersObject;
	public GameObject ronaldo;
	public GameObject playersOwnRonaldo;
	public GameObject explosion;
	public GameObject pickup;
	Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();

	public SpotlightControl control;

	private int spawnRepel;

	public AudioSource audioSource;
	public AudioClip smokeBomb;
	public float smokeBombVolume;

	//Vibration variables
	private float vibrationTimer = 0.5f;

	void Start() {
		control = this.transform.parent.GetComponent<SpotlightControl>();
		spawnRepel = managersObject.GetComponent<GameManager>().spawnDistanceFromSpotlight;
		playersOwnRonaldo = this.transform.parent.transform.parent.GetChild(1).gameObject;
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject gameObject = collider.gameObject;
		IEnumerator enumerator = null;

		//Second conditional changed to != check if the Ronaldo found is not their own 
		if (gameObject.tag == "Real Ronaldo" && gameObject.GetComponent<NPCTraits>().player != control.player) {
			Debug.Log("Ronaldo entered spotlight!");
			enumerator = MoveRonaldo(2.0f, gameObject);
		}

		if (gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo entered spotlight!");
			enumerator = MoveFakeRonaldo(2.0f, gameObject);
		}

		if (gameObject.CompareTag("Item")) {
			ItemEffect effect = gameObject.GetComponent<ItemEffect>();

			if (effect.CanActivate(this.transform.parent.transform.parent)) {
				enumerator = ActivateItem(1f, gameObject);
			}
		}

		if (enumerator != null) {
			coroutines.Add(gameObject, StartCoroutine(enumerator));
		}

	}

	private void OnTriggerExit(Collider collider) {
		GameObject gameObject = collider.gameObject;
		if (coroutines.ContainsKey(gameObject)) {
			Coroutine coroutine = coroutines[gameObject];
			StopCoroutine(coroutine);
			coroutines.Remove(gameObject);
		}

		if (gameObject.tag == "Real Ronaldo" && gameObject.GetComponent<NPCTraits>().player != control.player) {
			Debug.Log("Ronaldo exited spotlight!");
		}

		if (gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo exited spotlight!");
		}
	}

	IEnumerator MoveRonaldo(float delay, GameObject ronaldo) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, ronaldo.transform.position, Quaternion.identity) as GameObject;

		audioSource.PlayOneShot(smokeBomb, smokeBombVolume);
		GameManager.CaptureRonaldo(ronaldo, control.player);

		coroutines.Remove(ronaldo);
	}

	IEnumerator MoveFakeRonaldo(float delay, GameObject fakeRonaldo) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(explosion, fakeRonaldo.transform.position, Quaternion.identity) as GameObject;
		fakeRonaldo.transform.position = GameManager.GetRandomPointOnMap(fakeRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));

		audioSource.PlayOneShot(smokeBomb, smokeBombVolume);
		//EZGM.GetEZGamepad(control.player).HapticsOverTime(vibrationTimer, 1, 0);
		Debug.Log("Fake Ronaldo has moved!");

		// Subtract points from corresponding spotlight
		ScoreManager.ChangeScoreBy(control.player, -20);

		coroutines.Remove(fakeRonaldo);
	}

	IEnumerator ActivateItem(float delay, GameObject item) {
		// Wait
		yield return new WaitForSeconds(delay);
		// Hide item
		item.transform.position = new Vector3(1000, 1000, 1000);
		// Spawn pickup effect
		Instantiate(pickup, item.transform.position, Quaternion.identity);
		// Activate the item
		ItemEffect effect = item.GetComponent<ItemEffect>();
		effect.Activate(GameManager.GetPlayerGroup(control.player));
		// Remove the coroutine
		coroutines.Remove(item);
	}
}
