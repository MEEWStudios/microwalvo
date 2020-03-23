using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detection : MonoBehaviour
{
	public GameObject ronaldo;
	public GameObject fakeRonaldo;
	public GameObject explosion;
	public GameObject pickup;
	IEnumerator coroutine;

	void Start () {
	    fakeRonaldo = GameObject.Find("Fake Ronaldo");
		ronaldo = GameObject.Find("Ronaldo");
	}


	private void OnTriggerEnter(Collider col) {

		if(col.gameObject.name == "Ronaldo")
		{
            Debug.Log("Ronaldo entered spotlight!");
			coroutine = MoveRonaldo(3.0f);
			StartCoroutine(coroutine);
		}

		if(col.gameObject.name == "Fake Ronaldo")
		{
            Debug.Log("Fake Ronaldo entered spotlight!");
			coroutine = MoveFakeRonaldo(3.0f);
			StartCoroutine(coroutine);
		}

		if(col.gameObject.name == "Good Item")
		{
            Debug.Log("Good Item entered spotlight!");
			coroutine = RemoveItem(1.0f, col.gameObject);
			StartCoroutine(coroutine);
		}

		if(col.gameObject.name == "Bad Item")
		{
            Debug.Log("Bad Item entered spotlight!");
			coroutine = RemoveItem(1.0f, col.gameObject);
			StartCoroutine(coroutine);
		}

	}

	private void OnTriggerExit(Collider other) {

		if(other.gameObject.name == "Ronaldo")
		{
            Debug.Log("Ronaldo exited spotlight!");
			StopCoroutine(coroutine);
		}

		if(other.gameObject.name == "Fake Ronaldo")
		{
            Debug.Log("Fake Ronaldo exited spotlight!");
			StopCoroutine(coroutine);
		}

		if(other.gameObject.name == "Good Item")
		{
            Debug.Log("Good Item exited spotlight!");
			StopCoroutine(coroutine);
		}

		if(other.gameObject.name == "Bad Item")
		{
            Debug.Log("Bad Item exited spotlight!");
			StopCoroutine(coroutine);
		}
	}

	bool CheckSpawnPosition(Vector3 position, Vector3 size) {
		Vector3 spotlightPosition = this.gameObject.transform.position;
		Vector3 spotlightSize = this.gameObject.GetComponent<MeshCollider>().bounds.size;

		return !(Mathf.Abs(spotlightPosition.x - position.x) * 2 < (spotlightSize.x + size.x)) ||
		 !(Mathf.Abs(spotlightPosition.z - spotlightPosition.z) * 2 < (spotlightSize.z + size.z));
	}

	IEnumerator MoveRonaldo(float delay) {
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
	}

	IEnumerator MoveFakeRonaldo(float delay) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Vector3 position = fakeRonaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		position.x = Random.Range(position.x - 15, position.x + 15);
		position.z = Random.Range(position.z - 15, position.z + 15);
		fakeRonaldo.transform.position = position;
		Debug.Log("Fake Ronaldo has moved!");
	}

	IEnumerator RemoveItem(float delay, GameObject itemToRemove) {
		yield return new WaitForSeconds(delay);
		GameObject effect = Instantiate(pickup, transform.position, Quaternion.identity) as GameObject;
		if (itemToRemove.name == "Good Item"){
		    Debug.Log("Good Item Removed!");
		}

        if(itemToRemove.name == "Bad Item"){
		    Debug.Log("Bad Item Removed!");
		}

        Destroy(itemToRemove);

	}

	// Update is called once per frame
	void Update()
    {

    }
}
