using UnityEngine;
using System.Collections;

public abstract class ItemEffect : MonoBehaviour {
	public abstract float EffectDuration { get; }

	private float bobOffset;
	private Vector3 initialPosition;

	// Use this for initialization
	void OnEnable() {
		bobOffset = Random.Range(0f, 1f);
		initialPosition = this.transform.position;
	}

	// Update is called once per frame
	void Update() {
		Vector3 position = this.transform.position;

		// Make the item bob
		position.y = initialPosition.y + (Mathf.Sin((Time.timeSinceLevelLoad + bobOffset) * 3) * 0.8f);
		this.transform.position = position;
	}

	public void Activate(GameObject playerObject) {
		StartCoroutine(DoEffect(playerObject));
	}

	public virtual bool CanActivate(GameObject playerObject) {
		return true;
	}

	public virtual IEnumerator DoEffect(GameObject playerObject) {
		ApplyEffect(playerObject);
		yield return new WaitForSeconds(EffectDuration);
		RemoveEffect(playerObject);
		// Hack to perform some final actions before the object is removed
		yield return new WaitForSeconds(10);
		Destroy(this.gameObject);
	}

	public abstract void ApplyEffect(GameObject playerObject);

	public abstract void RemoveEffect(GameObject playerObject);
}
