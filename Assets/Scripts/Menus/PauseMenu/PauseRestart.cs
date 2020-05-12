using UnityEngine;

public class PauseRestart : MenuButton {
	public override void Trigger() {
		transform.parent.gameObject.SetActive(false);

		GameManager.ResetRound();
		ScoreManager.ResetScores();
		GameManager.StartRound();
		MenuController.Clear();
	}
}
