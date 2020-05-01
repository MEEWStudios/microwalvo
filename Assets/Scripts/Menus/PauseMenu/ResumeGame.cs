using UnityEngine;
using System.Collections;

public class ResumeGame : MenuButton {
	public override void Trigger() {
		GameManager.ResumeRound();
	}
}
