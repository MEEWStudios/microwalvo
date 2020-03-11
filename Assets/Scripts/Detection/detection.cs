﻿using System.Collections;
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
			coroutine = moveFakeRonaldo(3.0f);
			StartCoroutine(coroutine);
		}

		if(col.gameObject.name == "Good Item")
		{
            Debug.Log("Good Item entered spotlight!");
			coroutine = removeItem(1.0f, col.gameObject);
			StartCoroutine(coroutine);
		}

		if(col.gameObject.name == "Bad Item")
		{
            Debug.Log("Bad Item entered spotlight!");
			coroutine = removeItem(1.0f, col.gameObject);
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

	IEnumerator MoveRonaldo(float delay) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Vector3 position = ronaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		position.x = Random.Range(position.x - 15, position.x + 15);
		position.z = Random.Range(position.z - 15, position.z + 15);
		ronaldo.transform.position = position;
		Debug.Log("Ronaldo has moved!");
	}

	IEnumerator moveFakeRonaldo(float delay) {
		yield return new WaitForSeconds(delay);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity) as GameObject;
		Vector3 position = fakeRonaldo.transform.position;
		// The current plane we have is 10 x 10 x 10
		position.x = Random.Range(position.x - 15, position.x + 15);
		position.z = Random.Range(position.z - 15, position.z + 15);
		fakeRonaldo.transform.position = position;
		Debug.Log("Fake Ronaldo has moved!");
	}

	IEnumerator removeItem(float delay, GameObject itemToRemove) {
		yield return new WaitForSeconds(delay);
		GameObject item = Instantiate(pickup, transform.position, Quaternion.identity) as GameObject;
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