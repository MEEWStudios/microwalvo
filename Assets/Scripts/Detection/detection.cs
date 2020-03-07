using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detection : MonoBehaviour
{
	public GameObject ronaldo;
	IEnumerator coroutine;

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
		}

		if(col.gameObject.name == "Good Item")
		{
            Debug.Log("Good Item entered spotlight!");
		}

		if(col.gameObject.name == "Bad Item")
		{
            Debug.Log("Bad Item entered spotlight!");
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
            Debug.Log(" Fake Ronaldo exited spotlight!");
		}

		if(other.gameObject.name == "Good Item")
		{
            Debug.Log("Good Item exited spotlight!");
		}

		if(other.gameObject.name == "Bad Item")
		{
            Debug.Log("Bad Item exited spotlight!");
		}
	}

	IEnumerator MoveRonaldo(float delay) {
		yield return new WaitForSeconds(delay);
		Vector3 position = ronaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		position.x = Random.Range(-5, 5);
		//position.y = Random.Range(-5, 5);
		//position.z = Random.Range(-5, 5);
		ronaldo.transform.position = position;
		Debug.Log("Ronaldo has moved!");
	}

	// Update is called once per frame
	void Update()
    {

    }
}