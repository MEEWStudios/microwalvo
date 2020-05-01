using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : PlayerControl {
	public static readonly float DEFAULT_SPEED = 12;
	// Joystick needs to be at least this far away from the center to activate [0, 1]
	public float joystickActivationDistance = 0.05f;
	public float speed = DEFAULT_SPEED;
	public Player player;

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		if (gamepad.selectButton.justPressed) {
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		} else if (gamepad.startButton.justPressed) {
			if (GameManager.IsRoundPaused()) {
				GameManager.ResumeRound();
			} else {
				GameManager.PauseRound();
			}
		}

		if (!GameManager.IsRoundPaused()) {
			float xAxis = gamepad.leftJoystick.xAxis;
			float yAxis = gamepad.leftJoystick.yAxis;
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
}
