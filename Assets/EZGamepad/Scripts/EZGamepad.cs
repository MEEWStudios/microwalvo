/*
(C) 2020 Ronnie Kauanoe.
*/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

/// <summary> Simplified Gamepad class. Contains button and joystick values. </summary>
public class EZGamepad {

    /// <summary> Dpad up EZButton. </summary>
    public EZButton dpadUp;
    /// <summary> Dpad down EZButton. </summary>
    public EZButton dpadDown;
    /// <summary> Dpad left EZButton. </summary>
    public EZButton dpadLeft;
    /// <summary> Dpad right EZButton. </summary>
    public EZButton dpadRight;

    /// <summary> Top face EZButton. </summary>
    public EZButton buttonNorth;
    /// <summary> Right face EZButton. </summary>
    public EZButton buttonEast;
    /// <summary> Bottom face EZButton. </summary>
    public EZButton buttonSouth;
    /// <summary> Left face EZButton. </summary>
    public EZButton buttonWest;

    /// <summary> Left stick press EZButton. </summary>
    public EZButton leftStickPress;
    /// <summary> Right stick press EZButton. </summary>
    public EZButton rightStickPress;

    /// <summary> Left shoulder EZButton. </summary>
    public EZButton leftShoulder;
    /// <summary> Right shoulder EZButton. </summary>
    public EZButton rightShoulder;

    /// <summary> Left trigger EZButton. </summary>
    public EZButton leftTrigger;
    /// <summary> Right trigger EZButton. </summary>
    public EZButton rightTrigger;

    /// <summary> Center right EZButton. </summary>
    public EZButton startButton;
    /// <summary> Center left EZButton. </summary>
    public EZButton selectButton;

    /// <summary> Array of EZButtons. Useful for iterating through all EZButtons. </summary>
    public EZButton[] ezButtons = new EZButton[16];

    /// <summary> Left EZJoystick. </summary>
    public EZJoystick leftJoystick;
    /// <summary> Right EZJoystick. </summary>
    public EZJoystick rightJoystick;

    /// <summary> Array of EZJoysticks. Useful for iterating through all EZJoysticks. </summary>
    public EZJoystick[] ezJoysticks = new EZJoystick[2];

    /// <summary> Corresponding Unity Input System generic gamepad. </summary>
    private Gamepad gamepad = new Gamepad();

    /// <summary> Returns true if the EZGamepad was just connected. </summary>
    [HideInInspector]
    public bool justConnected = false;

    /// <summary> Returns true if the EZGamepad is connected. </summary>
    [HideInInspector]
    public bool isConnected = false;

    /// <summary> Returns true if the EZGamepad was just disconnected. </summary>
    [HideInInspector]
    public bool justDisconnected = false;

    /// <summary> Empty EZGamepad. </summary>
    public EZGamepad() { }

    /// <summary>
    ///     Creates EZGamepad using ButtonControls and StickControls from Gamepad class.
    /// </summary>
    /// <param name="gp"> Gamepad to derive ButtonControls and StickControls from. </param>
    public EZGamepad(Gamepad gp) {
        gamepad = gp;
        InitializeInput(gp);
    }

    /// <summary>
    ///     Assigns the gamepad's ButtonControls and StickControls to corresponding EZ inputs.
    /// </summary>
    /// <param name="gp"> Gamepad to derive ButtonControls and StickControls from. </param>
    private void InitializeInput(Gamepad gp) {

        //Dpad button assignments
        dpadUp = new EZButton(gp[GamepadButton.DpadUp]);
        dpadDown = new EZButton(gp[GamepadButton.DpadDown]);
        dpadLeft = new EZButton(gp[GamepadButton.DpadLeft]);
        dpadRight = new EZButton(gp[GamepadButton.DpadRight]);

        //Face button assignments
        buttonNorth = new EZButton(gp[GamepadButton.North]);
        buttonEast = new EZButton(gp[GamepadButton.East]);
        buttonSouth = new EZButton(gp[GamepadButton.South]);
        buttonWest = new EZButton(gp[GamepadButton.West]);

        //Stick button assignments
        leftStickPress = new EZButton(gp[GamepadButton.LeftStick]);
        rightStickPress = new EZButton(gp[GamepadButton.RightStick]);

        //Shoulder button assignments
        leftShoulder = new EZButton(gp[GamepadButton.LeftShoulder]);
        rightShoulder = new EZButton(gp[GamepadButton.RightShoulder]);

        //Trigger button assignments
        leftTrigger = new EZButton(gp[GamepadButton.LeftTrigger]);
        rightTrigger = new EZButton(gp[GamepadButton.RightTrigger]);

        //Center button assignments
        startButton = new EZButton(gp[GamepadButton.Start]);
        selectButton = new EZButton(gp[GamepadButton.Select]);

        //Joystick assignments
        leftJoystick = new EZJoystick(gp.leftStick);
        rightJoystick = new EZJoystick(gp.rightStick);

        //Populate EZButton array
        ezButtons = new[] {
            dpadUp, dpadDown, dpadLeft, dpadRight,
            buttonNorth, buttonEast, buttonSouth, buttonWest,
            leftStickPress, rightStickPress,
            leftShoulder, rightShoulder,
            leftTrigger, rightTrigger,
            startButton, selectButton
        };

        //Populate EZJoystick array
        ezJoysticks = new[] { leftJoystick, rightJoystick };
    }

    /// <summary>
    ///     Sets the haptic speeds of the left (low frequency) and right (high frequency) motor for
    ///     this device.
    /// </summary>
    /// <param name="lowFrequency">   Left motor frequency. </param>
    /// <param name="highFrequency">  Right motor frequency. </param>
    public void StartHaptics(float lowFrequency, float highFrequency) {
        gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
    }

    /// <summary> Pauses haptics for this device. </summary>
    public void PauseHaptics() {
        gamepad.PauseHaptics();
    }

    /// <summary>
    ///     Turns off haptics for this device.
    /// </summary>
    public void ResetHaptics() {
        gamepad.ResetHaptics();
    }

    /// <summary>
    ///     Resumes haptics on this device. Only resumes if haptics were paused and not reset.
    /// </summary>
    public void ResumeHaptics() {
        gamepad.ResumeHaptics();
    }

    /// <summary> Starts haptics on this device and stops after a set duration in seconds. </summary>
    /// <param name="duration">       Time interval the controller will vibrate. </param>
    /// <param name="lowFrequency">   Left motor frequency. </param>
    /// <param name="highFrequency">  Right motor frequency. </param>
    public void HapticsOverTime(float duration, float lowFrequency, float highFrequency) {
        EZGM.HapticsOverTime(this, duration, lowFrequency, highFrequency);
    }

    /// <summary> Returns the ID value given to the EZGamepad by Unity. </summary>
    /// <returns> The device identifier. </returns>
    public int GetDeviceID() {
        return gamepad.deviceId;
    }
}

/// <summary>
///     Simplified ButtonControl class. Contains "just pressed", "is pressed", and "just
///     released" values and press depth values (if applicable).
/// </summary>
public class EZButton {

    /// <summary> Returns true if the EZButton was just pressed this frame. </summary>
    public bool justPressed = false;
    /// <summary> Returns true if the EZButton is currently being held down. </summary>
    public bool isPressed = false;
    /// <summary> Returns true if the EZButton was just released this frame. </summary>
    public bool justReleased = false;

    /// <summary>
    ///     Press depth of this button this frame. Ranges from 0 to 1 for triggers. Either 0 or 1 for all others.
    /// </summary>
    public float pressDepth = 0f;

    /// <summary> Name of the EZButton. </summary>
    public string name = "";

    /// <summary> The button control. </summary>
    private ButtonControl buttonControl = new ButtonControl();

    /// <summary> Empty EZButton. </summary>
    public EZButton() { }

    /// <summary> Creates EZButton using corresponding ButtonControl. </summary>
    /// <param name="bc"> ButtonControl to derive name, pressed states, and press depth value from. </param>
    public EZButton(ButtonControl bc) {
        buttonControl = bc;
        name = bc.name;
    }

    /// <summary>
    ///     Updates this EZButton's "just pressed", "is pressed","just released"  and press depth values.
    /// </summary>
    public void UpdateButton() {
        justPressed = buttonControl.wasPressedThisFrame;
        isPressed = buttonControl.isPressed;
        justReleased = buttonControl.wasReleasedThisFrame;
        pressDepth = buttonControl.ReadValue();
    }
}

/// <summary> Simplified StickControl class. Contains X-Axis and Y-Axis values. </summary>
public class EZJoystick {

    /// <summary> The x-axis value (-1.0 to 1.0) of the EZJoystick. </summary>
    public float xAxis = 0f;
    /// <summary> The y-axis value (-1.0 to 1.0) of the EZJoystick. </summary>
    public float yAxis = 0f;

    /// <summary> Name of the EZJoystick. </summary>
    public string name = "";

    /// <summary> The stick control. </summary>
    private StickControl stickControl = new StickControl();
    /// <summary> The input vector. </summary>
    private Vector2 inputVector = Vector2.zero;

    /// <summary> Empty EZJoystick. </summary>
    public EZJoystick() { }

    /// <summary> Create EZJoystick using corresponding StickControl. </summary>
    /// <param name="sc"> StickControl to derive name and axis values from. </param>
    public EZJoystick(StickControl sc) {
        stickControl = sc;
        name = sc.name;
    }

    /// <summary> Updates this EZJoystick's x- and y-axis values. </summary>
    public void UpdateJoystick() {
        inputVector = stickControl.ReadValue();
        xAxis = inputVector.x;
        yAxis = inputVector.y;
    }
}