using UnityEngine;
using System.Collections;

public class Tutorial : MenuButton {
	public override void Trigger() {
		transform.parent.parent.gameObject.SetActive(false);
		GameObject howTo = transform.parent.parent.parent.Find("Tutorial").gameObject;
		howTo.SetActive(true);
		MenuController.EnterSubMenu(howTo.transform);
	}
}
