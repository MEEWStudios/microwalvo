using UnityEngine;

public class EndRestart : MenuButton {
	public override void Trigger() {
		GameManager.ResetRound();
		ScoreManager.ResetScores();
		GameManager.StartRound();
		MenuController.Clear();
	}
}
