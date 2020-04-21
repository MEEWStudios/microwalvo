using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
	public GameObject scoresObject;
	public Text winnerText;
	public bool scoreIsIncrementing = false;

	private Dictionary<Player, int> scores = new Dictionary<Player, int>();

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public void ResetScores(int playerCount) {
		scores.Clear();

		for (int i = 0; i < playerCount; i++) {
			scores.Add((Player) i, 0);
		}
	}

	public void ChangeScoreBy(Player player, int score) {

		scores[player] += score;
	

		if (scores[player] < 0) {
			scores[player] = 0;
		}

		GameObject text = scoresObject.transform.Find("P" + (((int) player) + 1)).Find("Score").gameObject;
		text.GetComponent<Text>().text = scores[player].ToString();
		
	}

	public IEnumerator StartIncreasingScoreBy(Player player, int score) {

		scoreIsIncrementing = true;

		while(scoreIsIncrementing) {
			scores[player] += score;
			yield return new WaitForSeconds(1.0f);

			if (scores[player] < 0) {
				scores[player] = 0;
			}

			GameObject text = scoresObject.transform.Find("P" + (((int) player) + 1)).Find("Score").gameObject;
			text.GetComponent<Text>().text = scores[player].ToString();
		}

	}

	public void DisplayResults() {
		int maxScore = 0;
		List<Player> players = new List<Player>();

		foreach (KeyValuePair<Player, int> pair in scores) {
			if (pair.Value > maxScore) {
				maxScore = pair.Value;
				players.Clear();
				players.Add(pair.Key);
			} else if (pair.Value == maxScore) {
				players.Add(pair.Key);
			}
		}

		if (players.Count > 1) {
			string text = "Players ";

			if (players.Count == scores.Count) {
				text = "All players";
			} else {
				for (int i = 0; i < players.Count; i++) {
					text += (((int) players[i]) + 1) + (i < players.Count - 2 ? " , " : i == players.Count - 2 ? " and " : "");
				}
			}
			text += " have tied!";
			winnerText.text = text;
		} else {
			winnerText.text = "Player " + (((int) players[0]) + 1) + " wins!";
		}
	}
}
