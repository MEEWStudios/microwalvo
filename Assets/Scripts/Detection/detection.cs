using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detection : MonoBehaviour
{
	public GameObject ronaldo;
	float x;

	private void OnTriggerEnter(Collider col) {
		Debug.Log("Detected!");

		//Vector3 position = ronaldo.transform.position;
		//position.x = Random.Range(-5, 5);
		//ronaldo.transform.position = position;
	}

	private void OnTriggerExit(Collider other) {
		Debug.Log("Undetected!");
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
