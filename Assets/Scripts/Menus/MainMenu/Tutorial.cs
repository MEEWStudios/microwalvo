using UnityEngine;

public class Tutorial : MenuButton {
	public override void Trigger() {
		transform.parent.parent.gameObject.SetActive(false);
		Transform howTo = transform.parent.parent.parent.Find("Tutorial");
		howTo.gameObject.SetActive(true);
		MenuController.EnterSubMenu(howTo);
	}
}
