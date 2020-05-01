using UnityEngine;
using System.Collections;

public class ReturnFromTutorial : MenuButton {
	public override void Trigger() {
		transform.parent.gameObject.SetActive(false);
		transform.parent.parent.Find("MainMenu").gameObject.SetActive(true);
		MenuController.ReturnToParentMenu();
	}
}
