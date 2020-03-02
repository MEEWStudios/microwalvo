using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UIAligner : ScriptableWizard {
	public int topPadding = 10;
	public int rightPadding = 10;
	public int bottomPadding = 10;
	public int leftPadding = 10;
	public UIHorizontalAlignment childHorizontalAlignment;
	public UIVerticalAlignment childVerticalAlignment;

	public enum UIHorizontalAlignment { Left, Center, Right, Stretch };
	public enum UIVerticalAlignment { Top, Center, Bottom, Stretch };

	[MenuItem("UI Helper/Reposition Selected UI Elements")]
	static void CreateWizard() {
		ScriptableWizard.DisplayWizard<UIAligner>("Reposition UI Elements", "Reposition");
	}

	private void OnWizardCreate() {

	}
}
