using UnityEngine;

public class GrowSpotlight : ItemEffect {
	public override float EffectDuration { get { return 4; } }

	private float scaleChange = 1.25f;
	private GameObject spotlight;

	public override void ApplyEffect(GameObject playerObject) {
		spotlight = playerObject.transform.Find("Spotlight").gameObject;
		Vector3 scale = spotlight.transform.localScale;
		scale.x *= scaleChange;
		scale.z *= scaleChange;
		spotlight.transform.localScale = scale;
	}

	public override void RemoveEffect(GameObject playerObject) {
		Vector3 scale = spotlight.transform.localScale;
		scale.x /= scaleChange;
		scale.z /= scaleChange;
		spotlight.transform.localScale = scale;
	}
}
