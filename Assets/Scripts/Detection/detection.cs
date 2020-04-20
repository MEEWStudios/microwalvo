using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class detection : MonoBehaviour {
	public GameObject managersObject;
	public GameObject ronaldo;
	public GameObject explosion;
	public GameObject pickup;
	Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
	IEnumerator coroutine;

	public SpotlightControl control;
	private ScoreManager scoreManager;
	private GameManager gameManager;

	private int spawnRepel;

	public AudioSource pointIncreaseSound;
	public AudioSource pointDecreaseSound;
	public AudioSource smokeBombSound;

	void Start() {
		control = this.transform.parent.GetComponent<SpotlightControl>();
		scoreManager = managersObject.GetComponent<ScoreManager>();
		gameManager = managersObject.GetComponent<GameManager>();
		spawnRepel = managersObject.GetComponent<GameManager>().spawnRepel;
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject gameObject = collider.gameObject;
		IEnumerator enumerator = null;

		if (gameObject.tag == "Real Ronaldo" && gameObject.GetComponent<NPCTraits>().player == control.player) {
			Debug.Log("Ronaldo entered spotlight!");
			enumerator = MoveRonaldo(2.0f, gameObject);
		}

		if (gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo entered spotlight!");
			enumerator = MoveFakeRonaldo(2.0f, gameObject);
		}

		if (gameObject.CompareTag("Item")) {
			enumerator = ActivateItem(1f, gameObject);
		}

		if(gameObject.CompareTag("Key")) {
			enumerator = ActivateKey(1f, gameObject);
			Debug.Log("Key in spotlight");
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

		if (gameObject.tag == "Real Ronaldo" && gameObject.GetComponent<NPCTraits>().player == control.player) {
			Debug.Log("Ronaldo exited spotlight!");
		}

		if (gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo exited spotlight!");
		}
	}

	//Eventually change to despawn Ronaldo instead
	IEnumerator MoveRonaldo(float delay, GameObject ronaldo) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, ronaldo.transform.position, Quaternion.identity) as GameObject;
		ronaldo.transform.position = GameManager.GetRandomPointOnMap(ronaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));

		smokeBombSound.Play();
		pointIncreaseSound.Play();
		
		//Spawn Key
		StartCoroutine(gameManager.spawnKey());

		// Add points to the corresponding spotlight's score
		StartCoroutine(scoreManager.ChangeScoreBy(control.player, 1)); 

		coroutines.Remove(ronaldo);
	}

	IEnumerator MoveFakeRonaldo(float delay, GameObject fakeRonaldo) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(explosion, fakeRonaldo.transform.position, Quaternion.identity) as GameObject;
		fakeRonaldo.transform.position = GameManager.GetRandomPointOnMap(fakeRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));

		smokeBombSound.Play();
		pointDecreaseSound.Play();
		Debug.Log("Fake Ronaldo has moved!");

		// Subtract points from corresponding spotlight
		scoreManager.ChangeScoreBy(control.player, -1);

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
		effect.Activate(GameManager.GetPlayerObject(control.player));
		// Remove the coroutine
		coroutines.Remove(item);
	}

	IEnumerator ActivateKey(float delay, GameObject item) {
		// Wait
		yield return new WaitForSeconds(delay);
		// Hide item
		item.transform.position = new Vector3(1000, 1000, 1000);
		// Spawn pickup effect
		Instantiate(pickup, item.transform.position, Quaternion.identity);
		// Activate the item
		//ItemEffect effect = item.GetComponent<ItemEffect>();
		//effect.Activate(GameManager.GetPlayerObject(control.player));
		scoreManager.scoreIsIncrementing = false;
		// Remove the coroutine
		coroutines.Remove(item);
	}
}
