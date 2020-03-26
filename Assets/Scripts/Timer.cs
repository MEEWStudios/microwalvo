using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
	private int time = 60;
	private bool running = false;
	private float timeCounter = 0;

	// Start is called before the first frame update
	void Start() {
		Run();
	}

	// Update is called once per frame
	void Update() {
		if (running) {
			timeCounter += Time.deltaTime;
			this.GetComponent<Text>().text = "Time: " + TimestampToString(time - timeCounter);
		}
	}

	void Run() {
		running = true;
	}

	string TimestampToString(float timestamp) {
		int minutes = Mathf.FloorToInt(timestamp / 60);
		timestamp -= minutes * 60;
		int seconds = Mathf.FloorToInt(timestamp);

		return minutes + ":" + (seconds < 10 ? "0" + seconds : "" + seconds);
	}
}
