using UnityEngine;

public class JailRelease : ItemEffect {
	public override float EffectDuration { get { return 4; } }

	public override bool CanActivate(GameObject playerObject) {
		return !playerObject.transform.Find("Ronaldo").gameObject.activeSelf;
	}

	public override void ApplyEffect(GameObject playerObject) {
		GameObject.Find("Managers").GetComponent<ScoreManager>().scoreIsIncrementing = false;
		playerObject.transform.Find("Ronaldo").gameObject.SetActive(true);
	}

	public override void RemoveEffect(GameObject playerObject) {
	}
}
