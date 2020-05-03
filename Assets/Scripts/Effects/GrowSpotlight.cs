using UnityEngine;

public class GrowSpotlight : ItemEffect {
	public override float EffectDuration { get { return 8; } }

	private float scaleChange = 1.5f;
	private float areaLightPositionChange = 1.5f;
	private Transform spotlight;
	private Transform areaLight;
	public AudioSource spotlightChangeSound;

	public override void ApplyEffect(Transform playerGroup) {
		spotlight = playerGroup.Find("Spotlight");
		Vector3 scale = spotlight.localScale;
		scale.x *= scaleChange;
		scale.z *= scaleChange;
		spotlight.localScale = scale;

		areaLight = spotlight.Find("Area Light");
		Vector3 areaLightPosition = areaLight.position;
		areaLightPosition.y *= areaLightPositionChange;
		areaLight.position = areaLightPosition;
		areaLight.GetComponent<Light>().intensity *= 2;

		spotlightChangeSound.Play();
	}

	public override void RemoveEffect(Transform playerGroup) {
		Vector3 scale = spotlight.localScale;
		scale.x /= scaleChange;
		scale.z /= scaleChange;
		spotlight.localScale = scale;

		Vector3 areaLightPosition = areaLight.position;
		areaLightPosition.y /= areaLightPositionChange;
		areaLight.position = areaLightPosition;
		areaLight.GetComponent<Light>().intensity /= 2;

		spotlightChangeSound.Play();
	}
}
