using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : PlayerControl {

	public GameObject mainMenu;
	public GameObject panel;
	public GameObject backText;
	public Button play;
	public Button instruct;
	public Button quit;
	Image imageComponent;
	public Sprite title;
	public Sprite tutorial;

	// Use this for initialization
	void Start() {
		ControlManager.RegisterGlobalControl(this);
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		imageComponent = GetComponent<Image>();
		// Start game
		if (gamepad.buttonSouth.justPressed) {
			mainMenu = GameObject.Find("MainMenu");
			mainMenu.SetActive(false);
			GameManager.ResetRound();
			ScoreManager.ResetScores();
			GameManager.StartRound();
		}
		if (gamepad.buttonWest.justPressed) {
			backText.SetActive(true);
			imageComponent.sprite = tutorial;
			panel.SetActive(false);
		}
		if (gamepad.buttonEast.justPressed && imageComponent.sprite == tutorial) {
			Debug.Log("button pressed!");
			panel.SetActive(true);
			backText.SetActive(false);
			imageComponent.sprite = title;
		}
		if (gamepad.buttonEast.justPressed) {
			Application.Quit();
		}

	}
}
