using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour {

	List<EZGamepad> gamepads = new List<EZGamepad>();
	List<PlayerControl> controlList = new List<PlayerControl>();
	List<PlayerControl> activeControls = new List<PlayerControl>();
	// Only used after player count is locked
	public int playerCount = -1;

	// Update is called once per frame
	void Update() {
		if (playerCount != -1) {
			// Reconnect any disconnected gamepads
			for (int i = 0; i < playerCount; i++) {
				if (gamepads[i] == null || !gamepads[i].isConnected) {
					gamepads[i] = EZGM.GetEZGamepad((Player) i);
				}
			}

			// Call ProcessGamepadInput on all active controls
			for (int i = 0; i < playerCount; i++) {
				if (activeControls[i] != null && gamepads[i] != null) {
					activeControls[i].ProcessGamepadInput(gamepads[i]);
				}
			}
		}

		if (playerCount != -1) {
			for (int i = 0; i < playerCount; i++) {
				if (gamepads[i].buttonSouth.justPressed) {
					GivePlayerControl(i, controlList[0]);
				}
			}
		}
	}

	public void LockPlayers() {
		gamepads.Clear();
		activeControls.Clear();
		playerCount = EZGM.EZGamepadCount();

		for (int i = 0; i < playerCount; i++) {
			gamepads.Add(EZGM.GetEZGamepad((Player) i));
			activeControls.Add(null);
		}
	}

	public void ReleasePlayerLock() {
		gamepads.Clear();
		activeControls.Clear();
		playerCount = -1;
	}

	public void RegisterControl(PlayerControl control) {
		if (controlList.IndexOf(control) == -1) {
			controlList.Add(control);
		}
	}

	public void UnregisterControl(PlayerControl control) {
		for (int i = 0; i < activeControls.Count; i++) {
			if (control == activeControls[i]) {
				activeControls[i] = null;
			}
		}

		controlList.Remove(control);
	}

	public void GivePlayerControl(int player, PlayerControl control) {
		for (int i = 0; i < activeControls.Count; i++) {
			if (control == activeControls[i]) {
				activeControls[i] = null;
			}
		}
		activeControls[player] = control;
	}

	public void RemovePlayerControl(int player) {
		activeControls[player] = null;
	}

	// Debug menu
	void OnGUI() {
		GUI.Box(new Rect(Screen.width - 150 - 10, 10, 150, 150), "Player Control");

		GUI.enabled = playerCount == -1;
		if (GUI.Button(new Rect(Screen.width - 150, 40, 130, 20), "Lock Players")) {
			LockPlayers();
		}

		GUI.enabled = playerCount != -1;
		if (GUI.Button(new Rect(Screen.width - 150, 70, 130, 20), "Release Lock")) {
			ReleasePlayerLock();
		}
		
		GUI.enabled = activeControls.Count > 0 && activeControls[0] == null;
		if (GUI.Button(new Rect(Screen.width - 150, 100, 130, 20), "Give Control")) {
			GivePlayerControl(0, controlList[0]);
		}

		GUI.enabled = activeControls.Count > 0 && activeControls[0] != null;
		if (GUI.Button(new Rect(Screen.width - 150, 130, 130, 20), "Remove Control")) {
			RemovePlayerControl(0);
		}
	}
}
