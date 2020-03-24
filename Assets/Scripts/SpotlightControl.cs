using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : PlayerControl {
	public enum Axis {
		Horizontal, Vertical
	}

	// degrees [0, 180]
	public Team team;
	public Axis controlAxis;
	public float joystickActivationAngle = 40;
	// Joystick needs to be at least this far away from the center to activate [0, 1]
	public float joystickActivationDistance = 0.85f;
	public double degreeDensity = 0.005;
	public GameObject managersObject;

	public UnityEngine.UI.Text distanceText;
	public UnityEngine.UI.Text wheelAngleText;
	public UnityEngine.UI.Text angleText;
	public UnityEngine.UI.Text differenceText;
	public UnityEngine.UI.Text offsetText;
	public UnityEngine.UI.Text newWheelAngleText;

	public GameObject spotlight;

	private Vector2 transformOffset;
	private ControlManager controlManager;
	private float wheelAngleDegrees = 0f;
	private float lastAngleDegrees = 0f;

	// Start is called before the first frame update
	void Start() {
		transformOffset = new Vector2(spotlight.transform.position.x, spotlight.transform.position.y);

		controlManager = managersObject.GetComponent<ControlManager>();
		controlManager.RegisterControl(this, team);
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		float xAxis = gamepad.rightJoystick.xAxis;
		float yAxis = gamepad.rightJoystick.yAxis;
		float joystickAngleRadians = Mathf.Atan2(yAxis, xAxis);
		float joystickAngleDegrees = joystickAngleRadians * 180 / Mathf.PI + 180;
		//float newWheelAngle = joystickAngleDegrees - offsetAngleDegrees;
		// Get distance from center of joystick [0, 1]
		float joystickDistance = Mathf.Sqrt(Mathf.Pow(yAxis, 2) + Mathf.Pow(xAxis, 2));

		if (distanceText != null) {
			distanceText.text = "Distance: " + joystickDistance;
		}
		if (joystickDistance < joystickActivationDistance) {
			// End of rotation interaction; wait for new rotation interaction
			lastAngleDegrees = Mathf.Infinity;
			return;
		} else if (lastAngleDegrees == Mathf.Infinity) {
			// New rotation interaction; wait for next input
			lastAngleDegrees = joystickAngleRadians;
			return;
		}

		if (Mathf.Abs(GetAngleDifference(lastAngleDegrees, joystickAngleDegrees)) > joystickActivationAngle) {
			// New rotation interaction; wait for next input
			lastAngleDegrees = joystickAngleDegrees;
			return;
		}

		wheelAngleDegrees -= GetAngleDifference(joystickAngleDegrees, lastAngleDegrees);

		this.transform.rotation = Quaternion.Euler(wheelAngleDegrees, -90, -90);

		if (wheelAngleText != null) {
			wheelAngleText.text = "Wheel Angle: " + wheelAngleDegrees;
		}
		if (angleText != null) {
			angleText.text = "Joystick Angle: " + joystickAngleDegrees;
		}
		if (differenceText != null) {
			differenceText.text = "Angle Difference: " + GetAngleDifference(joystickAngleDegrees, wheelAngleDegrees);
		}
		if (newWheelAngleText != null) {
			newWheelAngleText.text = "Last Angle: " + lastAngleDegrees;
		}

		lastAngleDegrees = joystickAngleDegrees;
		MoveSpotlight();
	}

	private bool AngleWithinThreshold(float angleDegrees1, float angleDegrees2, float threshold) {
		return Mathf.Abs(angleDegrees1 - angleDegrees2) <= threshold || Mathf.Abs(Mathf.Max(angleDegrees1, angleDegrees2) - 360 - Mathf.Min(angleDegrees1, angleDegrees2)) <= threshold;
	}

	// https://gamedev.stackexchange.com/questions/4467/comparing-angles-and-working-out-the-difference/169509#169509
	private float GetAngleDifference(float angleDegrees1, float angleDegrees2) {
		return (angleDegrees2 - angleDegrees1 + (360 + 180)) % 360 - 180;
	}

	private int GetQuadrantFromAngleDegree(float angleDegrees) {
		// Get angleDegree as [0, 360)
		angleDegrees = Mathf.Abs(angleDegrees % 360);

		return 1 + Mathf.FloorToInt(angleDegrees / 90);
	}

	private void MoveSpotlight() {
		Vector3 newPosition = spotlight.transform.position;
		if (controlAxis == Axis.Horizontal) {
			newPosition.x = (float) (-wheelAngleDegrees * degreeDensity) + transformOffset.x;
		} else {
			newPosition.z = (float) (-wheelAngleDegrees * degreeDensity) + transformOffset.y;
		}

		spotlight.transform.position = newPosition;
	}
}
