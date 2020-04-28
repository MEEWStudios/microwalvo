using UnityEngine;

public class SpeedBoost : ItemEffect {
	public override float EffectDuration { get { return 8; } }
	public AudioSource speedUp;
	public AudioSource slowDown;

	private SpotlightControl gamepadControl;
	private KeyboardControl keyboardControl;

	public override void ApplyEffect(Transform playerGroup) {
		gamepadControl = playerGroup.Find("Spotlight").GetComponent<SpotlightControl>();
		keyboardControl = playerGroup.Find("Spotlight").GetComponent<KeyboardControl>();
		gamepadControl.speed *= 2;
		keyboardControl.speed *= 2;
		speedUp.Play();
	}

	public override void RemoveEffect(Transform playerGroup) {
		gamepadControl.speed /= 2;
		keyboardControl.speed /= 2;
		slowDown.Play();
	}
}
