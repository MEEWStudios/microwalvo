using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class detection : MonoBehaviour {
	public GameObject managersObject;
	public GameObject ronaldo;
	public GameObject playersOwnRonaldo;
	public GameObject explosion;
	public GameObject pickup;
	Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
	Dictionary<Player, Player> captureTracker = new Dictionary<Player, Player>();
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

			if (effect.CanActivate(this.transform.parent.transform.parent.gameObject)) {
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

		//Capturer : Captured
		captureTracker.Add(control.player, ronaldo.GetComponent<NPCTraits>().player);

		//Turn off Ronaldo when found
		ronaldo.SetActive(false);
		ronaldo.transform.position = GameManager.GetRandomPointOnMap(ronaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));
		smokeBombSound.Play();
		pointIncreaseSound.Play();
		// Add points to the corresponding spotlight's score over time
		StartCoroutine(scoreManager.StartIncreasingScoreBy(control.player, 1)); 
		
		//Spawn Key after a delay
		StartCoroutine(gameManager.spawnKey(5.0f));



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
}
