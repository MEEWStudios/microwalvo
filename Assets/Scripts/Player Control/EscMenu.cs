using UnityEngine;

public class EscMenu : MonoBehaviour {
	public bool escKeyWasDown = false;

	// Update is called once per frame
	void Update() {
		if (Input.GetKey(KeyCode.Escape) && !escKeyWasDown) {
			escKeyWasDown = true;
			if (GameManager.IsRoundInProgress()) {
				if (GameManager.IsRoundPaused()) {
					GameManager.ResumeRound();
				} else {
					GameManager.PauseRound();
				}
			} else {
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			}
		} else if (!Input.GetKey(KeyCode.Escape)) {
			escKeyWasDown = false;
		}
	}
}
