using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items : MonoBehaviour
{
	public GameObject goodItem;
	public int goodItemCount;
	public GameObject badItem;
	public int badItemCount;
	public GameObject animation;

    // Start is called before the first frame update
    void Start()
    {
		SpawnItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void SpawnItems() {
		Vector3 position = goodItem.transform.position;
		for (int i = 0; i < goodItemCount; i++) {
			position.x = Random.Range(-80, 80);
			position.z = Random.Range(-80, 80);
			GameObject GoodItem = Instantiate(goodItem, position, Quaternion.identity) as GameObject;
			GameObject sparkle = Instantiate(animation, transform.position, Quaternion.identity) as GameObject;
		}

		position = badItem.transform.position;
		for (int i = 0; i < badItemCount; i++) {
			position.x = Random.Range(-80, 80);
			position.z = Random.Range(-80, 80);
			GameObject BadItem = Instantiate(badItem, position, Quaternion.identity) as GameObject;
			GameObject sparkle2 = Instantiate(animation, transform.position, Quaternion.identity) as GameObject;
		}
	}
}
