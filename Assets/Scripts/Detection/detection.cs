using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detection : MonoBehaviour
{
	public GameObject ronaldo;
	float x;

	private void OnTriggerEnter(Collider col) {
		
		//Vector3 position = ronaldo.transform.position;
		//position.x = Random.Range(-5, 5);
		//ronaldo.transform.position = position;

		if(col.gameObject.name == "Ronaldo")
		{
            Debug.Log("Ronaldo entered spotlight!");
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

	// Update is called once per frame
	void Update()
    {
      
    }
}
