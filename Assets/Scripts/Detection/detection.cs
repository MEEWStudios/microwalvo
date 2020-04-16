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

	private int numItemsWaiting = 0;
	private int spawnRepel;

	public AudioSource pointIncreaseSound;
	public AudioSource pointDecreaseSound;
	public AudioSource slowDownSound;
	public AudioSource slowBackToNormalSound;
	public AudioSource speedUpSound;
	public AudioSource fastBackToNormalSound;
	public AudioSource smokeBombSound;

	Vector3 normalSpotlightSize = new Vector3(0,0,0);
	private bool initialzedSpotlightSize = false;

	void Start() {
		control = this.transform.parent.GetComponent<SpotlightControl>();
		scoreManager = managersObject.GetComponent<ScoreManager>();
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

		if (gameObject.tag == "GoodItem") {
			Debug.Log("Good Item entered spotlight!");
			enumerator = RemoveItem(1.0f, gameObject);
		}

		if (gameObject.tag == "BadItem") {
			Debug.Log("Bad Item entered spotlight!");
			enumerator = RemoveItem(1.0f, gameObject);
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

		if (gameObject.tag == "Good Item") {
			Debug.Log("Good Item exited spotlight!");
		}

		if (gameObject.tag == "Bad Item") {
			Debug.Log("Bad Item exited spotlight!");
		}
	}

	IEnumerator MoveRonaldo(float delay, GameObject ronaldo) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		ronaldo.transform.position = GameManager.GetRandomPointOnMap(ronaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));
		smokeBombSound.Play();
		pointIncreaseSound.Play();
		Debug.Log("Ronaldo has moved!");

		// Add points to the corresponding spotlight's score
		scoreManager.ChangeScoreBy(control.player, 1);

		coroutines.Remove(ronaldo);
	}

	IEnumerator MoveFakeRonaldo(float delay, GameObject fakeRonaldo) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		fakeRonaldo.transform.position = GameManager.GetRandomPointOnMap(fakeRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel)); ;
		smokeBombSound.Play();
		Debug.Log("Fake Ronaldo has moved!");
		pointDecreaseSound.Play();
		// Subtract points from corresponding spotlight
		scoreManager.ChangeScoreBy(control.player, -1);

		coroutines.Remove(fakeRonaldo);
	}

	IEnumerator RemoveItem(float delay, GameObject itemToRemove) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(pickup, transform.position, Quaternion.identity) as GameObject;

		string itemToRemoveTag = itemToRemove.tag;

		//StartCoroutine(modifySpotlightSpeed(itemToRemoveTag));
		StartCoroutine(modifySpotlightSpeed(itemToRemoveTag));

		Destroy(itemToRemove);

		coroutines.Remove(itemToRemove);
	}

	//Good item: larger spotlight, Bad Item: smaller spotlight
	IEnumerator modifySpotlightAppearance(string itemtoRemoveTag) {


		if (!initialzedSpotlightSize) {
			normalSpotlightSize.Set(transform.localScale.x, transform.localScale.y, transform.localScale.z);
			Debug.Log("x: " + transform.localScale.x + ", y: " + transform.localScale.y + ", z: " + transform.localScale.z);
			initialzedSpotlightSize = true;
		}


		if (itemtoRemoveTag == "GoodItem") {
			Debug.Log("Good Item Removed!");
			int scaleChange = 2;

			//Modify Spotlight parent's Y scale, and spotlightcollider's X and Z scales for appearance and collider effect
			transform.parent.transform.localScale = new Vector3(normalSpotlightSize.x, normalSpotlightSize.y * scaleChange, normalSpotlightSize.z);
			transform.localScale = new Vector3(normalSpotlightSize.x * scaleChange, normalSpotlightSize.y, normalSpotlightSize.z * scaleChange);

			//accounts for picking up multiple items in a row
			numItemsWaiting++;
			yield return new WaitForSeconds(5.0f);
			numItemsWaiting--;

			//back to normal after 5 seconds
			if(numItemsWaiting == 0) {
				transform.parent.transform.localScale = new Vector3(normalSpotlightSize.x, normalSpotlightSize.y / scaleChange, normalSpotlightSize.z);
				transform.localScale = new Vector3(normalSpotlightSize.x / scaleChange, normalSpotlightSize.y, normalSpotlightSize.z / scaleChange);
			}

		}

		else if (itemtoRemoveTag == "BadItem") {
			Debug.Log("Bad Item Removed!");
			float scaleChange = 4.0f;

			//Modify Spotlight parent's Y scale, and spotlightcollider's X and Z scales for appearance and collider effect
			transform.parent.transform.localScale = new Vector3(normalSpotlightSize.x, normalSpotlightSize.y / scaleChange, normalSpotlightSize.z);
			transform.localScale = new Vector3(normalSpotlightSize.x / scaleChange, normalSpotlightSize.y, normalSpotlightSize.z / scaleChange);

			//accounts for picking up multiple items in a row
			numItemsWaiting++;
			yield return new WaitForSeconds(5.0f);
			numItemsWaiting--;

			//back to normal
			if(numItemsWaiting == 0) {
				transform.parent.transform.localScale = new Vector3(normalSpotlightSize.x, normalSpotlightSize.y * scaleChange, normalSpotlightSize.z);
				transform.localScale = new Vector3(normalSpotlightSize.x * scaleChange, normalSpotlightSize.y, normalSpotlightSize.z * scaleChange);
			}


		}


	}

	//Good item: set faster speed, Bad Item: set slower speed
	IEnumerator modifySpotlightSpeed(string itemtoRemoveTag) {
		float fastMultiplier = 2;
		float slowMultiplier = 0.5f;
		GameObject spotlight = this.gameObject.transform.parent.gameObject;
		spotlight.TryGetComponent<KeyboardControl>(out KeyboardControl keyboardControl);
		SpotlightControl spotlightControl = spotlight.GetComponent<SpotlightControl>();

		if (itemtoRemoveTag == "GoodItem") {
			Debug.Log("Good Item Removed!");
			speedUpSound.Play();

			if (keyboardControl != null) {
				keyboardControl.speed = KeyboardControl.DEFAULT_SPEED * fastMultiplier;
			}
			spotlightControl.speed = SpotlightControl.DEFAULT_SPEED * fastMultiplier;
			

			//accounts for picking up multiple items in a row
			numItemsWaiting++;
			yield return new WaitForSeconds(5.0f);
			numItemsWaiting--;

			if(numItemsWaiting == 0) {
				if (keyboardControl != null) {
					fastBackToNormalSound.Play();
					keyboardControl.speed = KeyboardControl.DEFAULT_SPEED;
				}
				spotlightControl.speed = SpotlightControl.DEFAULT_SPEED;
			}


		}

		if (itemtoRemoveTag == "BadItem") {
			Debug.Log("Bad Item Removed!");
			slowDownSound.Play();

			if (keyboardControl != null) {
				keyboardControl.speed = KeyboardControl.DEFAULT_SPEED * slowMultiplier;
			}
			spotlightControl.speed = SpotlightControl.DEFAULT_SPEED * slowMultiplier;

			//accounts for picking up multiple items in a row
			numItemsWaiting++;
			yield return new WaitForSeconds(5.0f);
			numItemsWaiting--;

			if(numItemsWaiting == 0) {
				slowBackToNormalSound.Play();
				if (keyboardControl != null) {
					keyboardControl.speed = KeyboardControl.DEFAULT_SPEED;
				}
				spotlightControl.speed = SpotlightControl.DEFAULT_SPEED;
			}


		}
	}
}
