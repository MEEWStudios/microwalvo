using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuController : PlayerControl {
	public static MenuController instance;
	private static readonly float ACTIVATION_DISTANCE = 0.5f;
	private static readonly float ACTIVATION_DEBOUNCE = 0.25f;
	private static Stack<Transform> menuStack = new Stack<Transform>();
	private static List<MenuButton> buttons = new List<MenuButton>();
	private static float lastActivation;
	private static int activeButton;

	// Use this for initialization
	void Start() {
		instance = this;
	}

	public static void SetTopLevelMenu(Transform menu) {
		menuStack.Clear();
		menuStack.Push(menu);
		SetupMenu();
	}

	public static void EnterSubMenu(Transform menu) {
		menuStack.Push(menu);
		SetupMenu();
	}

	public static void ReturnToParentMenu() {
		menuStack.Pop();
		SetupMenu();
	}

	private static void SetupMenu() {
		Transform menu = menuStack.Peek();
		buttons.Clear();

		foreach (Transform child in menu) {
			MenuButton button = child.GetComponent<MenuButton>();

			if (button != null) {
				button.Blur();
				buttons.Add(button);
			}
		}

		if (buttons.Count == 0) {
			Debug.LogError("MenuController: No buttons in menu " + menu.name);
		}

		lastActivation = 0;
		activeButton = 0;
		buttons[activeButton].Focus();
	}

	public static void Clear() {
		menuStack.Clear();
		buttons.Clear();
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		if (buttons.Count == 0) {
			return;
		}

		float yAxis = gamepad.leftJoystick.yAxis;

		if (Mathf.Abs(yAxis) >= ACTIVATION_DISTANCE) {
			if (Time.timeSinceLevelLoad - lastActivation < ACTIVATION_DEBOUNCE) {
				return;
			}

			buttons[activeButton].Blur();

			// Calculate new button
			int change = -Mathf.RoundToInt(yAxis / Mathf.Abs(yAxis));
			activeButton += change;
			if (activeButton < 0) {
				activeButton = buttons.Count - 1;
			} else if (activeButton >= buttons.Count) {
				activeButton = 0;
			}

			buttons[activeButton].Focus();
			lastActivation = Time.timeSinceLevelLoad;
		} else if (gamepad.buttonSouth.justPressed) {
			buttons[activeButton].Trigger();
		}
	}
}
