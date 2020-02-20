using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnifyingGlass : MonoBehaviour {
	//Get Player One's gamepad
	EZGamepad gamepad = EZGM.GetEZGamepad(Player.One);
	//var speed : float = 1.0;
	//private float speed = 0.0625f;
	private float speed = 5f;

	// Start is called before the first frame update
	void Start() {
		Debug.Log("I'm alive!");
	}

	// Update is called once per frame
	void Update() {
		// Old joystick input system
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		transform.position += move * speed * Time.deltaTime;

		// Bad keyboard controls
		if (Input.GetKey(KeyCode.LeftArrow)) {
			Vector3 position = this.transform.position;
			position.x -= speed;
			this.transform.position = position;
		}
		if (Input.GetKey(KeyCode.RightArrow)) {
			Vector3 position = this.transform.position;
			position.x += speed;
			this.transform.position = position;
		}
		if (Input.GetKey(KeyCode.UpArrow)) {
			Vector3 position = this.transform.position;
			position.y += speed;
			this.transform.position = position;
		}
		if (Input.GetKey(KeyCode.DownArrow)) {
			Vector3 position = this.transform.position;
			position.y -= speed;
			this.transform.position = position;
		}

		//If Player One is connected...
		if (gamepad != null) {
			// EZJoystick manipulation
			Vector3 newPosition = this.transform.position;
			newPosition.x += gamepad.leftJoystick.xAxis * speed;
			newPosition.y += gamepad.leftJoystick.yAxis * speed;
			// Is this necessary?
			this.transform.position = newPosition;
		} else {
			//If the controller wasn't found, try to find the controller again.
			gamepad = EZGM.GetEZGamepad(Player.One);
		}
	}
}
