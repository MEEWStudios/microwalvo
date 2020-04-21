using UnityEngine;

public class JailRelease : ItemEffect {
	public override float EffectDuration { get { return 4; } }

	public override bool CanActivate(Transform playerGroup) {
		return !playerGroup.Find("Ronaldo").gameObject.activeSelf;
	}

	public override void ApplyEffect(Transform playerGroup) {
		GameObject.Find("Managers").GetComponent<ScoreManager>().scoreIsIncrementing = false;
		playerGroup.transform.Find("Ronaldo").gameObject.SetActive(true);
	}

	public override void RemoveEffect(Transform playerGroup) {
	}
}
