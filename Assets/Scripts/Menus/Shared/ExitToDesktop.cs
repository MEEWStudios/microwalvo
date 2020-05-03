public class ExitToDesktop : MenuButton {
	public override void Trigger() {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
