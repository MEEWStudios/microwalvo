/*
(C) 2020 Ronnie Kauanoe.
*/

using UnityEngine;

public class ControllerVisualizer : MonoBehaviour {

    /// <summary> Player enum identifier assigned to this controller GameObject. </summary>
    public Player player;

    /// <summary> Click checkbox in editor to have the controller vibrate when it connects. </summary>
    public bool vibrateOnConnect = false;

    /// <summary> Click checkbox in editor to display connection messages. </summary>
    public bool debugConnection = false;
    /// <summary> Click checkbox in editor to display stick axis information. </summary>
    public bool debugSticks = false;
    /// <summary> Click checkbox in editor to display button state information. </summary>
    public bool debugButtons = false;

    /// <summary> EZGamepad assigned to this controller GameObject. </summary>
    private EZGamepad gp;

    /// <summary> Dpad GameObjects with children Up, Down, Left, and Right. </summary>
    private GameObject dpad;

    /// <summary> Face button GameObjects. </summary>
    private GameObject buttons;

    /// <summary> Left stick GameObjects. </summary>
    private GameObject leftStick;
    /// <summary> Right stick GameObjects. </summary>
    private GameObject rightStick;

    /// <summary> Left shoulder GameObject. </summary>
    private GameObject leftShoulder;
    /// <summary> Left shoulder GameObject. </summary>
    private GameObject rightShoulder;

    /// <summary> Left trigger GameObject. </summary>
    private GameObject leftTrigger;
    /// <summary> Right trigger GameObject. </summary>
    private GameObject rightTrigger;

    /// <summary> Start (center right) button GameObject. </summary>
    private GameObject startButton;
    /// <summary> Select (center left) button GameObject. </summary>
    private GameObject selectButton;

    /// <summary> Displays the controller name if connected. </summary>
    private TextMesh tm;

    /// <summary> Color of the TextMesh if the controller is connected. </summary>
    private Color connectColor = new Color(13f / 255f, 142f / 255f, 0);
    /// <summary> Color of the TextMesh if the controller is disconnected. </summary>
    private Color disconnectColor = Color.red;

    // Start is called before the first frame update
    void Start() {

        //Save all the GameObjects to be used by this script
        dpad = transform.GetChild(0).gameObject;
        buttons = transform.GetChild(1).gameObject;
        leftStick = transform.GetChild(2).gameObject;
        rightStick = transform.GetChild(3).gameObject;
        leftShoulder = transform.GetChild(4).gameObject;
        rightShoulder = transform.GetChild(5).gameObject;
        leftTrigger = transform.GetChild(6).gameObject;
        rightTrigger = transform.GetChild(7).gameObject;
        startButton = transform.GetChild(8).gameObject;
        selectButton = transform.GetChild(9).gameObject;

        //Save the TextMesh
        tm = transform.GetChild(transform.childCount - 1).GetComponent<TextMesh>();

        //Obtain the EZGamepad of the player if connected
        gp = EZGM.GetEZGamepad(player);

        //The player is connected
        if (gp != null) {
            tm.text = "Controller " + player.ToString();
            tm.color = connectColor;
            //The player is not connected
        } else {
            tm.text = "Controller NaN";
            tm.color = disconnectColor;
        }
    }

    // Update is called once per frame
    void Update() {

        //If a controller has been assigned...
        if (gp != null) {
            if (EZGM.ConfigStoppedUpdating()) {
                if (gp.justConnected) {
                    if (debugConnection) {
                        Debug.Log(player.ToString() + " just connected");
                    }
                    //Update the TextMesh and color
                    tm.text = "Controller " + player.ToString();
                    tm.color = connectColor;

                    //Vibrate the controller on connect
                    if (vibrateOnConnect) {
                        gp.HapticsOverTime(2f, 0.8f, 3f);
                    }
                } else if (gp.isConnected) {
                    if (debugConnection) {
                        Debug.Log(player.ToString() + " is still connected");
                    }
                } else if (gp.justDisconnected) {
                    if (debugConnection) {
                        Debug.Log(player.ToString() + " just disconnected");
                    }
                    //The EZGamepad has diconnected and no EZGamepad is assigned to this controller
                    gp = null;
                    //Update TextMesh and color
                    tm.text = "Controller NaN";
                    tm.color = disconnectColor;
                }
            } else {
                //Update the controller's GameObjects if the EZGM is not updating
                Debug.Log(gp == null);

                UpdateAllSticks();
                UpdateDpad();
                UpdateFaceButtons();
                UpdateBackButtons();
                UpdateCenterButtons();
            }
        } else {
            //If there is not EZGamepad assigned to this controller, attempt to obtain one
            gp = EZGM.GetEZGamepad(player);
        }
    }

    /// <summary> Updates joystick position and button color state. </summary>
    private void UpdateAllSticks() {

        //Left stick updates
        UpdateStick(gp.leftJoystick, leftStick);
        UpdateButton(gp.leftStickPress, leftStick, 0);

        //Right stick updates
        UpdateStick(gp.rightJoystick, rightStick);
        UpdateButton(gp.rightStickPress, rightStick, 0);
    }

    /// <summary> Update Dpad button color states. </summary>
    private void UpdateDpad() {
        UpdateButton(gp.dpadUp, dpad, 0);
        UpdateButton(gp.dpadDown, dpad, 1);
        UpdateButton(gp.dpadLeft, dpad, 2);
        UpdateButton(gp.dpadRight, dpad, 3);
    }

    /// <summary> Update face button color states. </summary>
    private void UpdateFaceButtons() {
        UpdateButton(gp.buttonNorth, buttons, 0);
        UpdateButton(gp.buttonEast, buttons, 1);
        UpdateButton(gp.buttonSouth, buttons, 2);
        UpdateButton(gp.buttonWest, buttons, 3);
    }

    /// <summary> Update back button color states. </summary>
    private void UpdateBackButtons() {
        //Shoulder button updates
        UpdateButton(gp.leftShoulder, leftShoulder, -1);
        UpdateButton(gp.rightShoulder, rightShoulder, -1);

        //Trigger button updates
        UpdateButton(gp.leftTrigger, leftTrigger, -1);
        UpdateButton(gp.rightTrigger, rightTrigger, -1);
    }

    /// <summary> Update center button color states. </summary>
    private void UpdateCenterButtons() {
        UpdateButton(gp.startButton, startButton, -1);
        UpdateButton(gp.selectButton, selectButton, -1);
    }

    /// <summary> Updates joystick GameObject position to emulate real joystick position. </summary>
    /// <param name="joystick"> The EZJoystick. </param>
    /// <param name="stickObj"> The stick GameObject. </param>
    private void UpdateStick(EZJoystick joystick, GameObject stickObj) {
        Vector2 value = new Vector2(joystick.xAxis, joystick.yAxis);
        stickObj.transform.GetChild(0).localPosition = value;

        if (debugSticks) {
            Debug.Log("Player " + player.ToString() + " " + stickObj.name + ": " + value);
        }
    }

    /// <summary>
    ///     <para/>Updates button colors depending on state. [Blue: Just pressed] [Green: Is pressed]
    ///     [Red: Just released] [White: Not pressed]
    ///     <para>If the button to be updated is not a child of an empty GameObject, childIndex = -
    ///     1</para>
    /// </summary>
    /// <param name="button"> The EZButton to derive state values from. </param>
    /// <param name="obj"> The button GameObject </param>
    /// <param name="childIndex"> Zero-based index of the child. </param>
    private void UpdateButton(EZButton button, GameObject obj, int childIndex) {

        if (button.justPressed) {
            ChangeColor(obj, childIndex, Color.blue);
            if (debugButtons) {
                Debug.Log("Player " + player.ToString() + " just pressed " + button.name);
            }
        } else if (button.isPressed) {
            ChangeColor(obj, childIndex, Color.green);
            if (debugButtons) {
                Debug.Log("Player " + player.ToString() + " is pressing " + button.name);
            }
        } else if (button.justReleased) {
            ChangeColor(obj, childIndex, Color.red);
            if (debugButtons) {
                Debug.Log("Player " + player.ToString() + " just released " + button.name);
            }
        } else {
            ChangeColor(obj, childIndex, Color.white);
        }

        //Move the button GameObjects back if their corresponding EZButton is pushed down
        if (childIndex == -1) {
            Vector3 lpSelf = obj.transform.localPosition;
            obj.transform.localPosition = new Vector3(lpSelf.x, lpSelf.y, button.pressDepth);
        } else {
            Vector3 lpChild = obj.transform.GetChild(childIndex).localPosition;
            obj.transform.GetChild(childIndex).localPosition = new Vector3(lpChild.x, lpChild.y, button.pressDepth);
        }
    }

    /// <summary> Helper function to color button. </summary>
    /// <param name="obj"> The button GameObject. </param>
    /// <param name="childIndex"> Zero-based index of the child. </param>
    /// <param name="color"> Color dependent on current button state. </param>
    private void ChangeColor(GameObject obj, int childIndex, Color color) {
        if (childIndex == -1) {
            obj.GetComponent<MeshRenderer>().material.color = color;
        } else {
            obj.transform.GetChild(childIndex).GetComponent<MeshRenderer>().material.color = color;
        }
    }
}
