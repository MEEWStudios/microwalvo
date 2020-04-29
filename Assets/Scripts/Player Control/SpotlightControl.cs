﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : PlayerControl {
	public enum Axis {
		Horizontal, Vertical
	}

	public static readonly float DEFAULT_SPEED = 12;
	// Joystick needs to be at least this far away from the center to activate [0, 1]
	public float joystickActivationDistance = 0.05f;
	public float speed = DEFAULT_SPEED;
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
		newPosition.x += xAxis * speed * Time.deltaTime;
		newPosition.z += yAxis * speed * Time.deltaTime;
		this.transform.position = newPosition;
	}
}