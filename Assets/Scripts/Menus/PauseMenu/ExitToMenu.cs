using UnityEngine;
using System.Collections;

public class ExitToMenu : MenuButton {
	public override void Trigger() {
		transform.parent.gameObject.SetActive(false);
		GameManager.ResetRound();
		ScoreManager.ResetScores();
		Transform mainMenu = transform.parent.parent.Find("MainMenu");
		mainMenu.gameObject.SetActive(true);
		MenuController.SetTopLevelMenu(mainMenu.Find("Panel"));
	}
}
