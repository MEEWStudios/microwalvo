using UnityEngine;

public class EscMenu : MonoBehaviour {
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (GameManager.IsRoundInProgress()) {
				if (GameManager.IsRoundPaused()) {
					GameManager.ResumeRound();
				} else {
					GameManager.PauseRound();
				}
			} else {
//#if UNITY_EDITOR
//				UnityEditor.EditorApplication.isPlaying = false;
//#else
//				Application.Quit();
//#endif
			}
		}
	}
}
