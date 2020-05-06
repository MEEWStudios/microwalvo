using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class MenuButton : MonoBehaviour {
	private static Color FOCUS_COLOR = new Color(233 / 255f, 181 / 255f, 36 / 255f);
	private static Color BLUR_COLOR = new Color(1, 1, 1);
	private Image image;

	// Start is called before the first frame update
	void Awake() {
		image = GetComponent<Image>();

		// Add mouse handling
		if (GetComponent<EventTrigger>() == null) {
			EventTrigger trigger = gameObject.AddComponent<EventTrigger>();

			EventTrigger.Entry enterEntry = new EventTrigger.Entry();
			enterEntry.eventID = EventTriggerType.PointerEnter;
			enterEntry.callback.AddListener((data) => { MenuController.Focus(this); });
			trigger.triggers.Add(enterEntry);

			EventTrigger.Entry exitEntry = new EventTrigger.Entry();
			exitEntry.eventID = EventTriggerType.PointerExit;
			exitEntry.callback.AddListener((data) => { Blur(); });
			trigger.triggers.Add(exitEntry);

			EventTrigger.Entry clickEntry = new EventTrigger.Entry();
			clickEntry.eventID = EventTriggerType.PointerClick;
			clickEntry.callback.AddListener((data) => { Trigger(); });
			trigger.triggers.Add(clickEntry);
		}
	}

	public virtual void Focus() {
		image.color = FOCUS_COLOR;
	}

	public virtual void Blur() {
		image.color = BLUR_COLOR;
	}

	public abstract void Trigger();
}
