using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : PlayerControl {

	public GameObject menu;
	public Button play;
	public Button instruct;
	public Button quit;

	// Use this for initialization
	void Start() {
		ControlManager.RegisterGlobalControl(this);
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {

		// Start game
		if (gamepad.buttonSouth.justPressed) {
			menu = GameObject.Find("MainMenu");
			menu.SetActive(false);
			GameManager.ResetRound();
			ScoreManager.ResetScores();
			GameManager.StartRound();
		}
		if (gamepad.buttonEast.justPressed) {
			Application.Quit();
		}

	}
}
