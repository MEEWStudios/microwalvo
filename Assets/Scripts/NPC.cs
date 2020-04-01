using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
	public int fakeRonaldoCount;
	public GameObject fakeRonaldo;
	public int personCount;
	public GameObject person;


	// Start is called before the first frame update
	void Start() {
		SpawnNPCs();
	}

	// Update is called once per frame
	void Update() {

	}

	void SpawnNPCs() {
		Vector3 position = fakeRonaldo.transform.position;
		for (int i = 0; i < fakeRonaldoCount; i++) {
			position.x = Random.Range(-50, 50);
			position.z = Random.Range(-50, 50);
			GameObject NPC = Instantiate(fakeRonaldo, position, Quaternion.identity) as GameObject;
			NPC.tag = "Fake Ronaldo";
		}

		position = person.transform.position;
		for(int i = 0; i < personCount; i++) {
			position.x = Random.Range(-50, 50);
			position.z = Random.Range(-50, 50);
			GameObject NPC = Instantiate(person, position, Quaternion.identity) as GameObject;
		}

	}
}
