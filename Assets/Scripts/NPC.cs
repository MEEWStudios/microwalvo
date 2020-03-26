using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour {
	public GameObject person;

	// Start is called before the first frame update
	void Start() {
		SpawnNPC();
	}

	// Update is called once per frame
	void Update() {

	}

	void SpawnNPC() {
		Vector3 position = person.transform.position;
		for(int i = 0; i < 10; i++) {
			position.x = Random.Range(-40, 40);
			position.z = Random.Range(-40, 40);
			GameObject NPC = Instantiate(person, position, Quaternion.identity) as GameObject;
			NPC.tag = "Fake Ronaldo";
		}

	}
}
