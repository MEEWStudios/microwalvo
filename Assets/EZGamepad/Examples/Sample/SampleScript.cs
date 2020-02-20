/*
(C) 2020 Ronnie Kauanoe.
*/

using UnityEngine;

public class SampleScript : MonoBehaviour {

    //Get Player One's gamepad
    EZGamepad ezgp = EZGM.GetEZGamepad(Player.One);

    //TextMesh displaying the various inputs
    private TextMesh textMesh;

    // Start is called before the first frame update
    void Start() {

        //Get the TextMesh on this GameObject.
        textMesh = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update() {

        //Clear the TextMesh's text
        textMesh.text = "";

        //If Player One is connected...
        if (ezgp != null) {

            //Get the south button (Xbox One: A || PlayStation 4: Cross)
            EZButton buttonSouth = ezgp.buttonSouth;

            //Was the south button just pressed this frame?
            if (buttonSouth.justPressed) {
                textMesh.text = "South button just pressed";
            //Is the south button being held down?
            } else if (buttonSouth.isPressed) {
                textMesh.text = "South button is pressed";
            //Was the south button just released this frame?
            } else if (buttonSouth.justReleased) {
                textMesh.text = "South button just released";
            //Was the south button not pressed at all?
            } else {
                textMesh.text = "South button not pressed";
            }

            //Get the right trigger (Xbox One: Right Trigger || PlayStation 4: R2)
            EZButton rightTrigger = ezgp.rightTrigger;
            //Print the right trigger's depth
            textMesh.text += "\nRight trigger depth: " + rightTrigger.pressDepth;

            //Get the left joystick
            EZJoystick leftJoystick = ezgp.leftJoystick;
            textMesh.text += "\nLeft joystick:";
            //Print the left joystick's x-axis
            textMesh.text += "\n\tx=" + leftJoystick.xAxis;
            //Print the right joystick's y-axs
            textMesh.text += "\n\ty=" + leftJoystick.yAxis;

            //Get the left shoulder (Xbox One: Left Bumper || PlayStation 4: L1)
            EZButton leftShoulder = ezgp.leftShoulder;
            //Press the left shoulder button to start vibrating the controller
            if (leftShoulder.justPressed) {
                ezgp.StartHaptics(4f, 0.5f);
            //Release the left shoulder button to stop vibrating the controller
            } else if (leftShoulder.justReleased) {
                ezgp.ResetHaptics();
            }
            //Print the instructions
            textMesh.text += "\nPress and hold the left shoulder\nbutton to vibrate the controller";
        } else {
            //If the controller wasn't found, try to find the controller again.
            ezgp = EZGM.GetEZGamepad(Player.One);
        }
    }
}
