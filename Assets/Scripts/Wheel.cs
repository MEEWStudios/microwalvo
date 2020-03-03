using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : PlayerControl {
	// degrees [0, 180]
	public float joystickActivationAngle = 40;
	// Joystick needs to be at least this far away from the center to activate [0, 1]
	public float joystickActivationDistance = 0.85f;
	public GameObject managersObject;

	public UnityEngine.UI.Text distanceText;
	public UnityEngine.UI.Text wheelAngleText;
	public UnityEngine.UI.Text angleText;
	public UnityEngine.UI.Text differenceText;
	public UnityEngine.UI.Text offsetText;
	public UnityEngine.UI.Text newWheelAngleText;

	public GameObject spotlight;

	private ControlManager controlManager;
	private EZGamepad gamepad = EZGM.GetEZGamepad(Player.One);
	private float wheelAngleDegrees = 0f;
	private float lastAngleDegrees = 0f;

	// Start is called before the first frame update
	void Start() {
		controlManager = managersObject.GetComponent<ControlManager>();
		controlManager.RegisterControl(this);
	}

	// Update is called once per frame
	//void Update() {
	//	if (gamepad != null && gamepad.isConnected) {
	//		ProcessGamepadInput(gamepad);
	//	} else {
	//		gamepad = EZGM.GetEZGamepad(Player.One);
	//	}
	//}

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

		//if (joystickAngleDegrees - lastAngleDegrees > 0)
		//	Debug.Log(joystickAngleDegrees - lastAngleDegrees);
		//newWheelAngle += joystickAngleDegrees - lastAngleDegrees;
		wheelAngleDegrees -= GetAngleDifference(joystickAngleDegrees, lastAngleDegrees);
		//if (GetAngleDifference(joystickAngleDegrees, lastAngleDegrees) != 0)
		//	Debug.Log(GetAngleDifference(joystickAngleDegrees, lastAngleDegrees) + " " + joystickAngleDegrees + " " + lastAngleDegrees);

		//if (GetAngleDifference(newWheelAngle, wheelAngleDegrees) > angleActivationThreshold) {
		//	offsetAngleDegrees = joystickAngleDegrees - wheelAngleDegrees;
		//	newWheelAngle = joystickAngleDegrees - offsetAngleDegrees;
		//	lastAngleDegrees = 0;
		//}

		//Debug.Log(wheelAngleDegrees);
		//Debug.Log(newWheelAngle + " " + GetQuadrantFromAngleDegree(newWheelAngle));
		//if (GetQuadrantFromAngleDegree(wheelAngleDegrees) == 4 && GetQuadrantFromAngleDegree(newWheelAngle) == 1) {
		//	newWheelAngle += 360;
		//} else if (GetQuadrantFromAngleDegree(wheelAngleDegrees) == 1 && GetQuadrantFromAngleDegree(newWheelAngle) == 4) {
		//	newWheelAngle -= 360;
		//}

		this.transform.rotation = Quaternion.Euler(wheelAngleDegrees, -90, -90);
		//wheelAngleDegrees = newWheelAngle + rotationsAngle;

		//float angleDifference = (wheelAngleDegrees - (newWheelAngle));
		//Debug.Log(angleDifference);
		//Debug.Log("    Wheel Angle: " + wheelAngleDegrees);
		//Debug.Log("New Wheel Angle: " + (wheelAngleDegrees + angleDifference));
		//this.transform.rotation = Quaternion.Euler(wheelAngleDegrees + angleDifference, -90, -90);
		//wheelAngleDegrees += angleDifference;

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

		//if (angleDegree < 90) {
		//	return 1;
		//} else if (angleDegree < 180) {
		//	return 2;
		//} else if (angleDegree < 270) {
		//	return 3;
		//} else {
		//	return 4;
		//}

		return 1 + Mathf.FloorToInt(angleDegrees / 90);
	}

	private void MoveSpotlight() {
		double degreeDensity = 0.005;

		Vector3 newPosition = spotlight.transform.position;
		newPosition.x = (float) (-wheelAngleDegrees * degreeDensity);
		// Is this necessary?
		spotlight.transform.position = newPosition;
	}
}
