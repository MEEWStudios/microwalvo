using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemEffect : MonoBehaviour {
	public GameObject progressSource;
	public abstract float EffectDuration { get; }

	private float bobOffset;
	private Vector3 initialPosition;
	private RectTransform progressTransform;
	private GameObject ring;
	private Image image;
	private Detection detection = null;

	// Use this for initialization
	void OnEnable() {
		bobOffset = Random.Range(0f, 1f);
		initialPosition = transform.position;
		progressTransform = transform.Find("Progress").Find("Progress").GetComponent<RectTransform>();
		ring = transform.Find("Progress").Find("Ring").gameObject;
		image = transform.Find("Progress").Find("Progress").GetComponent<Image>();
	}

	// Update is called once per frame
	void Update() {
		Vector3 position = transform.position;

		// Make the item bob
		position.y = initialPosition.y + (Mathf.Sin((Time.timeSinceLevelLoad + bobOffset) * 3) * 0.8f);
		transform.position = position;
	}

	public void SetProgress(Detection detection, float progress) {
		if (this.detection != detection && this.detection != null) {
			return;
		}

		this.detection = detection;

		ring.SetActive(true);
		progressTransform.localScale = new Vector3(progress, progress, progress);
		image.color = PlayerColor.Get(detection.player) * new Color(1, 1, 1, progress);
	}

	public void CancelProgress(Detection detection) {
		if (this.detection != detection) {
			return;
		}

		ring.SetActive(false);
		this.detection = null;
		progressTransform.localScale = new Vector3(0, 0, 0);
	}

	public void Activate(Transform playerGroup) {
		progressTransform.localScale = new Vector3(0, 0, 0);
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
