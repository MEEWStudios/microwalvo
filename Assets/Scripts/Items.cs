using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour {
	public GameObject goodItem;
	public int goodItemCount;
	public GameObject badItem;
	public int badItemCount;
	public GameObject animation;

	// Start is called before the first frame update
	void Start() {
		SpawnItems();
	}

	// Update is called once per frame
	void Update() {

	}

	void SpawnItems() {
		for (int i = 0; i < goodItemCount; i++) {
			Vector3 position = GameManager.GetRandomPointOnMap(goodItem.transform.position.y);
			GameObject GoodItem = Instantiate(goodItem, position, Quaternion.identity) as GameObject;
			GameObject sparkle = Instantiate(animation, position, Quaternion.identity, GoodItem.transform) as GameObject;
			sparkle.transform.localScale = new Vector3(2, 2, 2);
		}

		for (int i = 0; i < badItemCount; i++) {
			Vector3 position = GameManager.GetRandomPointOnMap(goodItem.transform.position.y);
			GameObject BadItem = Instantiate(badItem, position, Quaternion.identity) as GameObject;
			GameObject sparkle = Instantiate(animation, position, Quaternion.identity, BadItem.transform) as GameObject;
			sparkle.transform.localScale = new Vector3(2, 2, 2);
		}
	}
}
