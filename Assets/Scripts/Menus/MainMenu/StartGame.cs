using UnityEngine;
using System.Collections;

public class StartGame : MenuButton {
	public override void Trigger() {
		transform.parent.parent.gameObject.SetActive(false);
		GameManager.ResetRound();
		ScoreManager.ResetScores();
		GameManager.StartRound();
		MenuController.Clear();
	}
}
