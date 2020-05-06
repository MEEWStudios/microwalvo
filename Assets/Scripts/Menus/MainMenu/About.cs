using UnityEngine;

public class About : MenuButton {
	public override void Trigger() {
		transform.parent.gameObject.SetActive(false);
		Transform about = transform.parent.parent.Find("About");
		about.gameObject.SetActive(true);
		MenuController.EnterSubMenu(about);
		MenuController.AddReturnHandler((parentMenu) => {
			about.gameObject.SetActive(false);
			parentMenu.gameObject.SetActive(true);
		});
	}
}
