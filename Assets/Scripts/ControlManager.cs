using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlManager : MonoBehaviour {
	Dictionary<Team, List<PlayerControl>> controlList = new Dictionary<Team, List<PlayerControl>>();
	Dictionary<Team, List<EZGamepad>> gamepads = new Dictionary<Team, List<EZGamepad>>();
	Dictionary<PlayerControl, EZGamepad> controlMappings = new Dictionary<PlayerControl, EZGamepad>();

	// Awake is called when the script instance is being loaded
	void Awake() {
		// Populate the dictionaries with empty lists
		foreach (Team team in Enum.GetValues(typeof(Team))) {
			controlList.Add(team, new List<PlayerControl>());
			gamepads.Add(team, new List<EZGamepad>());
		}
	}

	// Update is called once per frame
	void Update() {
		if (gamepads[(Team) 0].Count > 0) {
			// Reconnect any disconnected gamepads
			foreach (KeyValuePair<Team, List<EZGamepad>> teamGamepads in gamepads.ToList()) {
				for (int i = 0; i < teamGamepads.Value.Count; i++) {
					if (teamGamepads.Value[i] == null || !teamGamepads.Value[i].isConnected) {
						EZGamepad gamepad = EZGM.GetEZGamepad((Player) i);

						if (gamepad != null) {
							teamGamepads.Value[i] = gamepad;
						}
					}
				}
			}

			// Call ProcessGamepadInput on all active controls
			foreach (KeyValuePair<PlayerControl, EZGamepad> controlMap in controlMappings) {
				controlMap.Key.ProcessGamepadInput(controlMap.Value);
			}
		}

		if (gamepads[(Team) 0].Count > 0) {
			foreach (KeyValuePair<Team, List<EZGamepad>> teamGamepads in gamepads) {
				for (int i = 0; i < teamGamepads.Value.Count; i++) {
					if (teamGamepads.Value[i].buttonSouth.justPressed) {
						GivePlayerControl(teamGamepads.Value[i], controlList[teamGamepads.Key][0]);
					}
					if (teamGamepads.Value[i].buttonEast.justPressed) {
						GivePlayerControl(teamGamepads.Value[i], controlList[teamGamepads.Key][1]);
					}
				}
			}
		}
	}

	public void SetTeams(Dictionary<Team, List<EZGamepad>> teams) {
		gamepads = teams;
	}

	public void LockPlayers() {
		int playerCount = EZGM.EZGamepadCount();

		for (int i = 0; i < playerCount; i++) {
			EZGamepad gamepad = EZGM.GetEZGamepad((Player) i);
			int team = Mathf.FloorToInt((float) i / playerCount + 0.5f);
			Debug.Log(i + ", " + ((float) i / playerCount) + ", " + team);
			gamepads[(Team) team].Add(gamepad);
		}

		//controlMappings.Clear();
		//playerCount = EZGM.EZGamepadCount();

		//for (int i = 0; i < playerCount; i++) {
		//	gamepads.Add(EZGM.GetEZGamepad((Player) i));
		//	controlMappings.Add(null);
		//}
	}

	public void ReleasePlayerLock() {
		foreach (Team team in Enum.GetValues(typeof(Team))) {
			gamepads[team].Clear();
		}
		controlMappings.Clear();
	}

	public void RegisterControl(PlayerControl control, Team team) {
		if (controlList[team].IndexOf(control) == -1) {
			controlList[team].Add(control);
		}
	}

	public void UnregisterControl(PlayerControl control, Team team) {
		controlMappings.Remove(control);
		controlList[team].Remove(control);
	}

	public void GivePlayerControl(EZGamepad gamepad, PlayerControl control) {
		foreach (KeyValuePair<PlayerControl, EZGamepad> controlMap in controlMappings.ToList()) {
			if (controlMap.Value == gamepad) {
				controlMappings.Remove(controlMap.Key);
			}
		}

		controlMappings[control] = gamepad;
	}

	public void RemovePlayerControl(EZGamepad gamepad) {
		foreach (KeyValuePair<PlayerControl, EZGamepad> controlMap in controlMappings.ToList()) {
			if (controlMap.Value == gamepad) {
				controlMappings.Remove(controlMap.Key);
			}
		}
	}

	// Debug menu
	void OnGUI() {
		GUI.Box(new Rect(Screen.width - 150 - 10, 10, 150, 150), "Player Control");

		GUI.enabled = gamepads[(Team) 0].Count == 0;
		if (GUI.Button(new Rect(Screen.width - 150, 40, 130, 20), "Lock Players")) {
			LockPlayers();
		}

		GUI.enabled = gamepads[(Team) 0].Count > 0;
		if (GUI.Button(new Rect(Screen.width - 150, 70, 130, 20), "Release Lock")) {
			ReleasePlayerLock();
		}

		GUI.enabled = gamepads[(Team) 0].Count > 0 && !controlMappings.ContainsKey(controlList[Team.One][0]);
		if (GUI.Button(new Rect(Screen.width - 150, 100, 130, 20), "Give Control 1")) {
			GivePlayerControl(gamepads[Team.One][0], controlList[Team.One][0]);
		}

		GUI.enabled = gamepads[(Team) 0].Count > 0 && !controlMappings.ContainsKey(controlList[Team.One][1]);
		if (GUI.Button(new Rect(Screen.width - 150, 130, 130, 20), "Give Control 2")) {
			//RemovePlayerControl(gamepads[Team.One][0]);
			GivePlayerControl(gamepads[Team.One][0], controlList[Team.One][1]);
		}
	}
}
