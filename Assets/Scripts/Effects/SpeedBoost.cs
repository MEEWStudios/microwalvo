using UnityEngine;

public class SpeedBoost : ItemEffect {
	public override float EffectDuration { get { return 4; } }
	public AudioSource speedUp;
	public AudioSource slowDown;

	private SpotlightControl gamepadControl;
	private KeyboardControl keyboardControl;

	public override void ApplyEffect(GameObject playerObject) {
		gamepadControl = playerObject.transform.Find("Spotlight").GetComponent<SpotlightControl>();
		keyboardControl = playerObject.transform.Find("Spotlight").GetComponent<KeyboardControl>();
		gamepadControl.speed *= 2;
		keyboardControl.speed *= 2;
		speedUp.Play();
	}

	public override void RemoveEffect(GameObject playerObject) {
		gamepadControl.speed /= 2;
		keyboardControl.speed /= 2;
		slowDown.Play();
	}
}
