using UnityEngine;
using System.Collections;

public class OpenLink : PlayerControl {
	void OnEnable() {
		ControlManager.RegisterGlobalControl(this);
	}

	void OnDisable() {
		ControlManager.UnregisterGlobalControl(typeof(OpenLink));
	}

	public override void ProcessGamepadInput(EZGamepad gamepad) {
		if (gamepad.buttonSouth.justPressed) {
			OpenWebsiteLink();
		}
	}

	public void OpenWebsiteLink() {
		Application.OpenURL("https://meewstudios.github.io/microwalvo/");
	}
}
