using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlManager : MonoBehaviour {
	private static Dictionary<PlayerControl, List<EZGamepad>> controlMapping = new Dictionary<PlayerControl, List<EZGamepad>>();
	private static List<PlayerControl> globalControls = new List<PlayerControl>();

	// Awake is called when the script instance is being loaded
	void Awake() {

	}

	// Update is called once per frame
	void Update() {
		if (controlMapping.Count != EZGM.EZGamepadCount()) {
			// Reconnect any disconnected gamepads
			foreach (KeyValuePair<PlayerControl, List<EZGamepad>> map in controlMapping) {
				for (int i = 0; i < map.Value.Count; i++) {
					EZGamepad gamepad = map.Value[i];

					if (gamepad == null) {
						//Debug.Log("ITS NULL");
						continue;
					}

					if (!gamepad.isConnected) {
						//Debug.Log(EZGM.EZGamepadCount());
						//Debug.Log(gamepad);
						EZGamepad newGamepad = EZGM.GetEZGamepad((Player) gamepad.GetGamepadIndex());

						if (newGamepad != null) {
							//newControlMapping[map.Key] = gamepad;
							controlMapping[map.Key][i] = newGamepad;
						}
					}
				}
			}
		}

		// Update all controls
		// Call ProcessGamepadInput on all controls
		foreach (PlayerControl control in globalControls) {
			control.Update();
			for (int i = 0; i < EZGM.EZGamepadCount(); i++) {
				control.ProcessGamepadInput(EZGM.GetEZGamepad((Player) i));
			}
		}
		foreach (KeyValuePair<PlayerControl, List<EZGamepad>> controlMap in controlMapping) {
			controlMap.Key.Update();
			foreach (EZGamepad gamepad in controlMap.Value) {
				controlMap.Key.ProcessGamepadInput(gamepad);
			}
		}
	}

	public static void RegisterGlobalControl(PlayerControl control) {
		if (!globalControls.Contains(control)) {
			globalControls.Add(control);
		}
	}

	public static void UnregisterGlobalControl(Type type) {
		for (int i = globalControls.Count - 1; i >= 0; i--) {
			if (globalControls[i].GetType() == type) {
				globalControls.RemoveAt(i);
			}
		}
	}

	public static void RegisterControl(PlayerControl control, Player player) {
		if (!controlMapping.ContainsKey(control)) {
			if (EZGM.GetEZGamepad(player) == null) {
				return;
			}

			controlMapping.Add(control, new List<EZGamepad>() { EZGM.GetEZGamepad(player) });
		}
	}

	public static void UnregisterControl(PlayerControl control, Player player) {
		if (controlMapping.ContainsKey(control)) {
			if (EZGM.GetEZGamepad(player) == null) {
				return;
			}

			controlMapping[control].Remove(EZGM.GetEZGamepad(player));

			if (controlMapping[control].Count == 0) {
				controlMapping.Remove(control);
			}
		}
	}

	public static void UnregisterControls(Type type) {
		// Create instance of dictionary to modify
		Dictionary<PlayerControl, List<EZGamepad>> newControlMapping = new Dictionary<PlayerControl, List<EZGamepad>>(controlMapping);

		foreach (KeyValuePair<PlayerControl, List<EZGamepad>> map in controlMapping) {
			if (map.Key.GetType() == type) {
				newControlMapping.Remove(map.Key);
			}
		}

		controlMapping = newControlMapping;
	}
}
