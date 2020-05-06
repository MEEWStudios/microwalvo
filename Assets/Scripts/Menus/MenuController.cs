using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : PlayerControl {
	public delegate void OnReturnToParent(Transform parentMenu);

	private class MenuData {
		public Transform transform;
		public List<MenuButton> buttons = new List<MenuButton>();
		public int button = 0;
		public OnReturnToParent returnHandler = null;

		public MenuData(Transform transform) {
			this.transform = transform;
		}
	}

	public Sprite defaultSprite;
	public Font defaultFont;

	public static MenuController instance;
	private static readonly float ACTIVATION_DISTANCE = 0.5f;
	private static readonly float ACTIVATION_DEBOUNCE = 0.25f;
	private static readonly Stack<MenuData> stack = new Stack<MenuData>();
	private static MenuData activeMenu = null;
	private static float lastActivation;

	// Use this for initialization
	void Start() {
		instance = this;
	}

	override public void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (activeMenu?.returnHandler != null) {
				ReturnToParentMenu();
			}
		}
	}

	public static void SetTopLevelMenu(Transform menuTransform) {
		stack.Clear();
		OpenMenu(SetupMenu(menuTransform));
	}

	public static void AddReturnHandler(OnReturnToParent handler) {
		activeMenu.returnHandler += handler;
	}

	public static void EnterSubMenu(Transform menuTransform) {
		stack.Push(activeMenu);
		OpenMenu(SetupMenu(menuTransform));
	}

	public static void ReturnToParentMenu() {
		MenuData parent = stack.Count > 0 ? stack.Pop() : null;
		// Call handlers
		activeMenu.returnHandler(parent?.transform);

		if (parent == null) {
			Clear();
		} else {
			// Open parent menu
			OpenMenu(parent);
		}
	}

	public static void Focus(MenuButton button) {
		activeMenu.buttons[activeMenu.button].Blur();
		activeMenu.button = activeMenu.buttons.IndexOf(button);
		button.Focus();
	}

	private static MenuData SetupMenu(Transform menuTransform) {
		MenuData newMenu = new MenuData(menuTransform);

		foreach (Transform child in menuTransform) {
			MenuButton button = child.GetComponent<MenuButton>();

			if (button != null) {
				button.Blur();
				newMenu.buttons.Add(button);
			}
		}

		return newMenu;
	}

	private static void OpenMenu(MenuData menu) {
		activeMenu = menu;

		lastActivation = 0;

		if (activeMenu.buttons.Count > 0) {
			activeMenu.buttons[activeMenu.button].Focus();
		}
	}

	public static void Clear() {
		stack.Clear();
		activeMenu = null;
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		if (activeMenu == null) {
			return;
		}

		if (gamepad.buttonEast.justPressed) {
			if (activeMenu.returnHandler != null) {
				ReturnToParentMenu();

				return;
			}
		}

		if (activeMenu.buttons.Count == 0) {
			return;
		}

		float yAxis = gamepad.leftJoystick.yAxis;

		if (Mathf.Abs(yAxis) >= ACTIVATION_DISTANCE) {
			if (Time.timeSinceLevelLoad - lastActivation < ACTIVATION_DEBOUNCE) {
				return;
			}

			activeMenu.buttons[activeMenu.button].Blur();

			// Calculate new button
			int change = -Mathf.RoundToInt(yAxis / Mathf.Abs(yAxis));
			activeMenu.button += change;
			if (activeMenu.button < 0) {
				activeMenu.button = activeMenu.buttons.Count - 1;
			} else if (activeMenu.button >= activeMenu.buttons.Count) {
				activeMenu.button = 0;
			}

			activeMenu.buttons[activeMenu.button].Focus();
			lastActivation = Time.timeSinceLevelLoad;
		} else if (gamepad.buttonSouth.justPressed) {
			activeMenu.buttons[activeMenu.button].Trigger();
		}
	}
}
