using UnityEngine;
using UnityEngine.UI;

public class ProgressIndicator : MonoBehaviour {
	private Color TINT = new Color(0.7f, 0.7f, 0.7f);

	private RectTransform progressTransform;
	private Image image;
	private GameObject ring;
	private bool isInitialized = false;
	private Detection detection = null;

	// Use this for initialization
	public void Initialize(GameObject progressSource) {
		// Add the progress UI
		RectTransform canvas = (Instantiate(progressSource, transform, false) as GameObject).GetComponent<RectTransform>();
		// Set the UI position
		canvas.localPosition = new Vector3(0, 0, 0);
		Vector3 globalScale = canvas.lossyScale;
		canvas.localScale = new Vector3(1 / globalScale.x, 1 / globalScale.y, 1 / globalScale.z);
		// Assign fields
		progressTransform = canvas.Find("Progress").GetComponent<RectTransform>();
		image = canvas.Find("Progress").GetComponent<Image>();
		ring = canvas.Find("Ring").gameObject;
		isInitialized = true;
	}

	public void SetProgress(Detection detection, float progress) {
		if (!isInitialized) {
			return;
		}

		if (this.detection != detection && this.detection != null) {
			return;
		}

		this.detection = detection;

		ring.SetActive(true);
		progressTransform.localScale = new Vector3(progress, progress, progress);
		image.color = PlayerColor.Get(detection.player) * new Color(1.3f, 1.3f, 1.3f);
		ring.GetComponent<Image>().color = PlayerColor.Get(detection.player) * TINT;
	}

	public void ClearProgress(Detection detection) {
		if (!isInitialized) {
			return;
		}

		if (this.detection != detection) {
			return;
		}

		ring.SetActive(false);
		this.detection = null;
		progressTransform.localScale = new Vector3(0, 0, 0);
	}

	public void Clear() {
		if (!isInitialized) {
			return;
		}

		progressTransform.localScale = new Vector3(0, 0, 0);
	}
}
