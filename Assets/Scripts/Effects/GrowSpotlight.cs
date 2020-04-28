using UnityEngine;

public class GrowSpotlight : ItemEffect {
	public override float EffectDuration { get { return 8; } }

	private float scaleChange = 1.5f;
	private float areaLightPositionChange = 1.5f;
	private GameObject spotlight;
	private GameObject areaLight;
	public AudioSource spotlightChangeSound;

	public override void ApplyEffect(Transform playerGroup) {
		spotlight = playerGroup.Find("Spotlight").gameObject;
		Vector3 scale = spotlight.transform.localScale;
		scale.x *= scaleChange;
		scale.z *= scaleChange;
		spotlight.transform.localScale = scale;

		areaLight = spotlight.transform.Find("Area Light").gameObject;
		Vector3 areaLightPosition = areaLight.transform.position;
		areaLightPosition.y *= areaLightPositionChange;
		areaLight.transform.position = areaLightPosition;
		areaLight.GetComponent<Light>().intensity *= 2;

		spotlightChangeSound.Play();
	}

	public override void RemoveEffect(Transform playerGroup) {
		Vector3 scale = spotlight.transform.localScale;
		scale.x /= scaleChange;
		scale.z /= scaleChange;
		spotlight.transform.localScale = scale;

		Vector3 areaLightPosition = areaLight.transform.position;
		areaLightPosition.y /= areaLightPositionChange;
		areaLight.transform.position = areaLightPosition;
		areaLight.GetComponent<Light>().intensity /= 2;

		spotlightChangeSound.Play();
	}
}
