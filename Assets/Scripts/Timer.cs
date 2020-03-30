using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {
	private int time = 120;
	private bool running = false;
	private float timeCounter = 0;
	public Text timerText, p1Score, p2Score, p3Score, p4Score, winnerText;
	private int p1Num, p2Num, p3Num, p4Num;

	// Start is called before the first frame update
	void Start() {
		Run();
	}

	// Update is called once per frame
	void Update() {
		if (running) {
			timeCounter += Time.deltaTime;
			if((time - timeCounter) > 0) {
				timerText.text = TimestampToString(time - timeCounter);
			} else {
				running = false;
				timerText.text = "Finished!";

				p1Num = int.Parse(p1Score.text);
				p2Num = int.Parse(p2Score.text);
				p3Num = int.Parse(p3Score.text);
				p4Num = int.Parse(p4Score.text);

				if(p1Num == 0 && p2Num == 0 && p3Num == 0 && p4Num == 0) {
					winnerText.text = "Tie!";
				}
				else if(Math.Max(Math.Max(p1Num, p2Num), Math.Max(p3Num, p4Num)) == p1Num) {
					winnerText.text = "Player 1 Wins!";
				} else if(Math.Max(Math.Max(p1Num, p2Num), Math.Max(p3Num, p4Num)) == p2Num) {
					winnerText.text = "Player 2 Wins!";
				} else if(Math.Max(Math.Max(p1Num, p2Num), Math.Max(p3Num, p4Num)) == p3Num) {
					winnerText.text = "Player 3 Wins!";
				} else if(Math.Max(Math.Max(p1Num, p2Num), Math.Max(p3Num, p4Num)) == p4Num) {
					winnerText.text = "Player 4 Wins!";
				} 



			}
			
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
