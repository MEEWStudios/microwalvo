using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class detection : MonoBehaviour {
	public GameObject ronaldo;
	public GameObject explosion;
	public GameObject pickup;
	Dictionary<GameObject, Coroutine> coroutines = new Dictionary<GameObject, Coroutine>();
	IEnumerator coroutine;
	public Text p1Score, p2Score, p3Score, p4Score;

	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}


	private void OnTriggerEnter(Collider col) {
		GameObject gameObject = col.gameObject;
		IEnumerator enumerator = null;

		if (gameObject.tag == "Real Ronaldo") {
			Debug.Log("Ronaldo entered spotlight!");
			enumerator = MoveRonaldo(1.0f, gameObject);
		}

		if (gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo entered spotlight!");
			enumerator = MoveFakeRonaldo(1.0f, gameObject);
		}

		if (gameObject.name == "Good Item") {
			Debug.Log("Good Item entered spotlight!");
			enumerator = RemoveItem(1.0f, gameObject);
		}

		if (gameObject.name == "Bad Item") {
			Debug.Log("Bad Item entered spotlight!");
			enumerator = RemoveItem(1.0f, gameObject);
		}

		if (enumerator != null) {
			coroutines.Add(gameObject, StartCoroutine(enumerator));
		}

	}

	private void OnTriggerExit(Collider other) {
		GameObject gameObject = other.gameObject;
		if (coroutines.ContainsKey(gameObject)) {
			Coroutine coroutine = coroutines[gameObject];
			StopCoroutine(coroutine);
			coroutines.Remove(gameObject);
		}

		if (other.gameObject == ronaldo) {
			Debug.Log("Ronaldo exited spotlight!");
		}

		if (other.gameObject.tag == "Fake Ronaldo") {
			Debug.Log("Fake Ronaldo exited spotlight!");
		}

		if (other.gameObject.name == "Good Item") {
			Debug.Log("Good Item exited spotlight!");
		}

		if (other.gameObject.name == "Bad Item") {
			Debug.Log("Bad Item exited spotlight!");
		}
	}

	bool CheckSpawnPosition(Vector3 position, Vector3 size) {
		Vector3 spotlightPosition = this.gameObject.transform.position;
		Vector3 spotlightSize = this.gameObject.GetComponent<MeshCollider>().bounds.size;

		return !(Mathf.Abs(spotlightPosition.x - position.x) * 2 < (spotlightSize.x + size.x)) ||
		 !(Mathf.Abs(spotlightPosition.z - spotlightPosition.z) * 2 < (spotlightSize.z + size.z));
	}

	IEnumerator MoveRonaldo(float delay, GameObject ronaldo) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Vector3 position = ronaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		//do {
		position.x = Random.Range(position.x - 15, position.x + 15);
		position.z = Random.Range(position.z - 15, position.z + 15);
		//} while (CheckSpawnPosition(position, ronaldo.GetComponent<MeshCollider>().bounds.size));
		ronaldo.transform.position = position;
		Debug.Log("Ronaldo has moved!");
 		
		//add points to the corresponding spotlight's score
	    if(transform.parent.name == "Spotlight1") {
			p1Score.text = ((int.Parse(p1Score.text)) + 1).ToString();
		}
		if(transform.parent.name == "Spotlight2") {
			p2Score.text = ((int.Parse(p2Score.text)) + 1).ToString();
		}
		if(transform.parent.name == "Spotlight3") {
			p3Score.text = ((int.Parse(p3Score.text)) + 1).ToString();
		}
		if(transform.parent.name == "Spotlight4") {
			p4Score.text = ((int.Parse(p4Score.text)) + 1).ToString();
		}

		coroutines.Remove(ronaldo);
	}

	IEnumerator MoveFakeRonaldo(float delay, GameObject fakeRonaldo) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Vector3 position = fakeRonaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		position.x = Random.Range(position.x - 15, position.x + 15);
		position.z = Random.Range(position.z - 15, position.z + 15);
		fakeRonaldo.transform.position = position;
		Debug.Log("Fake Ronaldo has moved!");

		//Subtract points from corresponding spotlight
		if(transform.parent.name == "Spotlight1") {
			if(int.Parse(p1Score.text) > 0) {
				p1Score.text = ((int.Parse(p1Score.text)) - 1).ToString();
			}
		}
		if(transform.parent.name == "Spotlight2") {
			if(int.Parse(p2Score.text) > 0) {
				p2Score.text = ((int.Parse(p2Score.text)) - 1).ToString();
			}
		}
		if(transform.parent.name == "Spotlight3") {
			if(int.Parse(p3Score.text) > 0) {
				p3Score.text = ((int.Parse(p3Score.text)) - 1).ToString();
			}
		}
		if(transform.parent.name == "Spotlight4") {
			if(int.Parse(p4Score.text) > 0) {
				p4Score.text = ((int.Parse(p4Score.text)) - 1).ToString();
			}
		}

		coroutines.Remove(fakeRonaldo);
	}

	IEnumerator RemoveItem(float delay, GameObject itemToRemove) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(pickup, transform.position, Quaternion.identity) as GameObject;
		if (itemToRemove.name == "Good Item") {
			Debug.Log("Good Item Removed!");
			GameObject.Find("Spotlight1").GetComponent<KeyboardControl>().speed = 150;
		}

		if (itemToRemove.name == "Bad Item") {
			Debug.Log("Bad Item Removed!");
			GameObject.Find("Spotlight1").GetComponent<KeyboardControl>().speed = 5;
		}

		Destroy(itemToRemove);

		coroutines.Remove(itemToRemove);
	}
}
