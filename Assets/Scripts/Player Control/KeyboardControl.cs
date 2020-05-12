using UnityEngine;

public class KeyboardControl : MonoBehaviour {

	public static readonly float DEFAULT_SPEED = 12;
	public float speed = DEFAULT_SPEED;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if (!GameManager.IsRoundPaused()) {
			float xAxis = Input.GetAxis("Horizontal");
			float yAxis = Input.GetAxis("Vertical");

			Vector3 newPosition = this.transform.position;
			newPosition.x += xAxis * speed * Time.deltaTime;
			newPosition.z += yAxis * speed * Time.deltaTime;

			// Clamp values
			newPosition.x = Mathf.Clamp(newPosition.x, -117, 117);
			newPosition.z = Mathf.Clamp(newPosition.z, -50, 50);

			this.transform.position = newPosition;
		}
	}
}
