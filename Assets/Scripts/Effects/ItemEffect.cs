using System.Collections;
using UnityEngine;

public abstract class ItemEffect : MonoBehaviour {
	public abstract float EffectDuration { get; }

	private float bobOffset;
	private Vector3 initialPosition;

	// Use this for initialization
	void OnEnable() {
		bobOffset = Random.Range(0f, 1f);
		initialPosition = transform.position;
	}

	// Update is called once per frame
	void Update() {
		Vector3 position = transform.position;

		// Make the item bob
		position.y = initialPosition.y + (Mathf.Sin((Time.timeSinceLevelLoad + bobOffset) * 3) * 0.8f);
		transform.position = position;
	}

	public void Activate(Transform playerGroup) {
		StartCoroutine(DoEffect(playerGroup));
	}

	public virtual bool CanActivate(Transform playerGroup) {
		return true;
	}

	public virtual IEnumerator DoEffect(Transform playerGroup) {
		ApplyEffect(playerGroup);
		yield return new WaitForSeconds(EffectDuration);
		RemoveEffect(playerGroup);
		// Hack to perform some final actions before the object is removed
		yield return new WaitForSeconds(10);
		Destroy(this.gameObject);
	}

	public abstract void ApplyEffect(Transform playerGroup);

	public abstract void RemoveEffect(Transform playerGroup);
}
