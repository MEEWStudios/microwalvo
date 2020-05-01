using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour {

	public static readonly float DEFAULT_SPEED = 12;
	public float speed = DEFAULT_SPEED;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (!GameManager.IsRoundPaused()) {
			float x = Input.GetAxis("Horizontal");
			float z = Input.GetAxis("Vertical");

			x *= speed * Time.deltaTime;
			z *= speed * Time.deltaTime;

			transform.Translate(x, 0, z);
		}
	}
}
