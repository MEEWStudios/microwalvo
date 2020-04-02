using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : PlayerControl {
	public enum Axis {
		Horizontal, Vertical
	}

	// Joystick needs to be at least this far away from the center to activate [0, 1]
	public float joystickActivationDistance = 0.05f;
	public float speed = 1;
	public Player player;

	// Start is called before the first frame update
	void Start() {
		ControlManager.RegisterControl(this, player);
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		float xAxis = gamepad.rightJoystick.xAxis;
		float yAxis = gamepad.rightJoystick.yAxis;
		//float newWheelAngle = joystickAngleDegrees - offsetAngleDegrees;
		// Get distance from center of joystick [0, 1]
		float joystickDistance = Mathf.Sqrt(Mathf.Pow(yAxis, 2) + Mathf.Pow(xAxis, 2));

		if (joystickDistance < joystickActivationDistance) {
			return;
		}

		Vector3 newPosition = this.transform.position;
		newPosition.x += xAxis * speed;
		newPosition.z += yAxis * speed;
		this.transform.position = newPosition;
	}
}
