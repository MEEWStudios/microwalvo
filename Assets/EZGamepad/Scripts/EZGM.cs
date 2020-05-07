/*
(C) 2020 Ronnie Kauanoe.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary> Enumerated identifiers of the players. </summary>
public enum Player {
    One, Two, Three, Four
}

/// <summary> EZGamepadManager (aka EZGM). Updates and manages EZGampads. </summary>
public class EZGM : MonoBehaviour {

    /// <summary> Dictionary containing players and connected EZGamepads. </summary>
    private static Dictionary<Player, EZGamepad> ezGamepads = new Dictionary<Player, EZGamepad>();

    /// <summary> Check checkbox in editor to display connection messages. </summary>
    public bool debugConnections = false;

    /// <summary> Returns true if the EZGM is updating its dictionary. </summary>
    private static bool configUpdating = false;

    /// <summary> Number of currently connected controllers. </summary>
    private int currentCount = 0;
    /// <summary> Number of connected controllers from previous frame. </summary>
    private int prevCount = 0;

    /// <summary> True if fixed update called. </summary>
    private static bool fixedUpdateCalled = false;

    /// <summary> True to configuration stopped updating. </summary>
    private static bool configStoppedUpdating = false;

    /// <summary> The manager. </summary>
    private static EZGM manager;

    private void Awake() {
        ezGamepads.Clear();
        //Adds all connected EZGamepads to dictionary
        for (int i = 0; i < Gamepad.all.Count; i++) {
            ezGamepads.Add((Player)i, new EZGamepad(Gamepad.all[i], i));
        }

        manager = this;
    }

    private void Update() {

        currentCount = EZGamepadCount();

        //EZGamepads were either added or removed
        if (currentCount != prevCount) {
            //Only executes if the configuration just changed this frame
            if (!configUpdating) {
                configUpdating = true;
                UpdateEZGamepads();
            }
        }

        //Update EZGamepads if any are connected.
        if (currentCount != 0) {
            UpdateEZInput();
        }

        ////Press Escape to exit application.
        //if (Input.GetKey(KeyCode.Escape)) {
        //    Application.Quit();
        //}
    }

    private void LateUpdate() {
        if (fixedUpdateCalled) {
            DeleteEZGamepadHelper();
            fixedUpdateCalled = false;
            configUpdating = false;
            configStoppedUpdating = true;
            prevCount = currentCount;
        } else if (configUpdating) {
            StartCoroutine(UpdateConnectionStates());
            fixedUpdateCalled = true;
        } else if (configStoppedUpdating) {
            StartCoroutine(UpdateConnectionStates());
            configStoppedUpdating = false;
        }
    }

    /// <summary> Updates dictionary to store currently connected players and EZGamepads. </summary>
    private void UpdateEZGamepads() {

        //Create instance of dictionary to modify
        Dictionary<Player, EZGamepad> ezgpBuffer = new Dictionary<Player, EZGamepad>(ezGamepads);

        //More controllers than previous frame
        if (currentCount > prevCount) {

            if (debugConnections) {
                Debug.Log((currentCount - prevCount) + " EZGamepad" + (((currentCount - prevCount) > 1) ? "s" : "") + " connected");
            }

            //Index of the first new player added
            int nextIndex = prevCount;
            //Iterate through all players
            foreach (Player player in Enum.GetValues(typeof(Player))) {
                //If a player is already connected, do nothing
                if (ezgpBuffer.ContainsKey(player)) {
                    //Add only if there are still players left to add
                } else if (ezgpBuffer.Count < currentCount) {
                    ezgpBuffer.Add(player, new EZGamepad(Gamepad.all[nextIndex], nextIndex));
                    //Increment to next new player index
                    nextIndex++;
                }
            }

            //Less controllers than previous frame
        } else {
            int gpToRemove = 0;

            foreach (KeyValuePair<Player, EZGamepad> kvp in ezGamepads) {
                if (!IsEZGamepadConnected(kvp.Value)) {
                    gpToRemove++;
                }
            }

            if (debugConnections) {
                Debug.Log(gpToRemove + " EZGamepad" + ((gpToRemove > 1) ? "s" : "") + " disconnected");
            }
        }
        //Reassign dictionary
        ezGamepads = ezgpBuffer;
    }

    /// <summary> Updates the connection states of the EZGamepads. </summary>
    private static IEnumerator UpdateConnectionStates() {
        yield return new WaitForEndOfFrame();
        Dictionary<Player, EZGamepad> ezgpBuffer = new Dictionary<Player, EZGamepad>(ezGamepads);
        foreach (KeyValuePair<Player, EZGamepad> kvp in ezGamepads) {
            //Controller just added, just connected
            if (!kvp.Value.justConnected && !kvp.Value.isConnected && !kvp.Value.justDisconnected) {
                kvp.Value.justConnected = true;
                kvp.Value.isConnected = false;
                kvp.Value.justDisconnected = false;
                //Controller already added, is connected
            } else if (kvp.Value.justConnected) {
                kvp.Value.justConnected = false;
                kvp.Value.isConnected = true;
                kvp.Value.justDisconnected = false;
                //Controller just removed, just disconnected
            } else if (kvp.Value.isConnected && !IsEZGamepadConnected(kvp.Value)) {
                kvp.Value.justConnected = false;
                kvp.Value.isConnected = false;
                kvp.Value.justDisconnected = true;
            }
        }
    }

    /// <summary> Removes disconnected EZGamepads from the dictionary. </summary>
    private void DeleteEZGamepadHelper() {
        Dictionary<Player, EZGamepad> ezgpBuffer = new Dictionary<Player, EZGamepad>(ezGamepads);

        foreach (KeyValuePair<Player, EZGamepad> kvp in ezGamepads) {
            if (!IsEZGamepadConnected(kvp.Value)) {
                ezgpBuffer.Remove(kvp.Key);
            }
        }

        ezGamepads = ezgpBuffer;
    }

    /// <summary> Updates the EZButtons and EZJoysticks of all connected EZGamepads. </summary>
    private void UpdateEZInput() {
        //Iterate over every connected EZGamepad
        foreach (KeyValuePair<Player, EZGamepad> kvp in ezGamepads) {

            if (configUpdating) {
                //Skip EZGamepads that are in the process of being deleted
                if (!IsEZGamepadConnected(kvp.Value)) {
                    continue;
                }
            }

            //Update EZButtons
            foreach (EZButton button in kvp.Value.ezButtons) {
                button.UpdateButton();
            }

            //Update EZJoysticks
            foreach (EZJoystick joystick in kvp.Value.ezJoysticks) {
                joystick.UpdateJoystick();
            }
        }
    }

    /// <summary> Returns true if the EZGamepad is connected. </summary>
    /// <param name="ezgp"> EZGamepad to verify. </param>
    /// <returns> True if the EZGamepad is connected, false if not. </returns>
    private static bool IsEZGamepadConnected(EZGamepad ezgp) {

        if (ezgp == null) {
            return false;
        }

        int deviceID = ezgp.GetDeviceID();
        //Return true if the device ID can be found
        foreach (Gamepad gp in Gamepad.all) {
            if (gp.deviceId == deviceID) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    ///     Returns EZGamepad of specified player. Returns null if the player is not connected.
    /// </summary>
    /// <param name="p">  Player to retrieve EZGamepad from. </param>
    /// <returns> The EZGamepad of the specified player. </returns>
    public static EZGamepad GetEZGamepad(Player p) {
        try {
            return ezGamepads[p];
        } catch (KeyNotFoundException) {
            return null;
        }
    }

    /// <summary> Returns true if the EZGamepadManager is updating. </summary>
    /// <returns> True if the EZGM is updating, false otherwise. </returns>
    public static bool ConfigUpdating() {
        return configUpdating;
    }

    /// <summary> Returns true if the EZGamepadManager has stopped updating this frame. </summary>
    /// <returns> True if the EZGM stopped updating this frame, false otherwise. </returns>
    public static bool ConfigStoppedUpdating() {
        return configStoppedUpdating;
    }

    /// <summary> Returns the number of EZGamepads currently connected. </summary>
    /// <returns> The number of connected EZGamepads. </returns>
    public static int EZGamepadCount() {
        return Gamepad.all.Count;
    }

    /// <summary> Helper function for HapticsOverTime in EZGamepad. Do not call directly. </summary>
    /// <param name="gp">             The EZGamepad to vibrate. </param>
    /// <param name="duration">       The duration the EZGamepad will vibrate. </param>
    /// <param name="lowFrequency">   Frequency of the left motor. </param>
    /// <param name="highFrequency">  Frequency of the right motor. </param>
    public static void HapticsOverTime(EZGamepad gp, float duration, float lowFrequency, float highFrequency) {
        if (gp != null) {
            manager.StartCoroutine(HapticsCoroutine(gp, duration, lowFrequency, highFrequency));
        }
    }

    /// <summary>
    ///     Coroutine to start haptics on EZGamepad and wait to reset haptics over a set time
    ///     interval.
    /// </summary>
    /// <param name="gp">             The EZGamepad to vibrate. </param>
    /// <param name="duration">       The duration the EZGamepad will vibrate. </param>
    /// <param name="lowFrequency">   Frequency of the left motor. </param>
    /// <param name="highFrequency">  Frequency of the right motor. </param>
    /// <returns> An IEnumerator. </returns>
    private static IEnumerator HapticsCoroutine(EZGamepad gp, float duration, float lowFrequency, float highFrequency) {
        gp.ResetHaptics();
        gp.StartHaptics(lowFrequency, highFrequency);
        yield return new WaitForSeconds(duration);
        gp.ResetHaptics();
    }

    /// <summary> Disable the haptics when the application ends. </summary>
    private void OnApplicationQuit() {
        foreach (EZGamepad gp in ezGamepads.Values) {
            gp.ResetHaptics();
        }
    }
}
