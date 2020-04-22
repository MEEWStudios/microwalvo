using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
	private struct ScoreIncrementingCoroutine {
		public Coroutine coroutine;
		public int increment;

		public ScoreIncrementingCoroutine(Coroutine coroutine, int increment) {
			this.coroutine = coroutine;
			this.increment = increment;
		}
	}

	public AudioSource audioSource;
	public AudioClip pointIncreaseSound;
	public float pointIncreaseVolume;
	public AudioClip pointDecreaseSound;
	public float pointDecreaseVolume;
	public GameObject scoresObject;
	public Text winnerText;

	private static ScoreManager manager;
	private static Dictionary<Player, int> scores = new Dictionary<Player, int>();
	private static Dictionary<Player, List<ScoreIncrementingCoroutine>> incrementingScores = new Dictionary<Player, List<ScoreIncrementingCoroutine>>();

	// Start is called before the first frame update
	void Start() {
		manager = this;
	}

	// Update is called once per frame
	void Update() {

	}

	public static void ResetScores(int playerCount) {
		scores.Clear();
		incrementingScores.Clear();

		for (int i = 0; i < playerCount; i++) {
			scores.Add((Player) i, 0);
			incrementingScores.Add((Player) i, new List<ScoreIncrementingCoroutine>());
		}
	}

	public static void ChangeScoreBy(Player player, int score) {
		scores[player] += score;

		if (scores[player] < 0) {
			scores[player] = 0;
		}

		if (score > 0) {
			manager.audioSource.PlayOneShot(manager.pointIncreaseSound, manager.pointIncreaseVolume);
		} else if (score < 0) {
			manager.audioSource.PlayOneShot(manager.pointDecreaseSound, manager.pointDecreaseVolume);
		}

		GameObject text = manager.scoresObject.transform.Find("P" + (((int) player) + 1)).Find("Score").gameObject;
		text.GetComponent<Text>().text = scores[player].ToString();
	}

	public static void StartIncreasingScoreBy(Player player, int score) {
		incrementingScores[player].Add(new ScoreIncrementingCoroutine(manager.StartCoroutine(IncrementScoreBy(player, score)), score));
	}

	public static void StopIncreasingScoreBy(Player player, int score) {
		List<ScoreIncrementingCoroutine> routines = incrementingScores[player];

		foreach (ScoreIncrementingCoroutine incrementingCoroutine in routines) {
			if (incrementingCoroutine.increment == score) {
				manager.StopCoroutine(incrementingCoroutine.coroutine);
				routines.Remove(incrementingCoroutine);

				return;
			}
		}
	}

	private static IEnumerator IncrementScoreBy(Player player, int score) {
		while (true) {
			ChangeScoreBy(player, score);

			yield return new WaitForSeconds(0.5f);
		}
	}

	public static void DisplayResults() {
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
			manager.winnerText.text = text;
		} else {
			manager.winnerText.text = "Player " + (((int) players[0]) + 1) + " wins!";
		}
	}
}
