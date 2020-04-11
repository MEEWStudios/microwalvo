using UnityEngine;
using System.Collections;

public abstract class ItemEffect {
	public virtual IEnumerator Activate(float duration) {
		ApplyEffect();
		yield return new WaitForSeconds(duration);
		RemoveEffect();
	}

	public abstract void ApplyEffect();

	public abstract void RemoveEffect();
}
