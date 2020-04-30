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

	[Header("UI Configuration")]
	public Transform prefabSource;
	public Transform roundPanel;
	public Text winnerText;
	public int padding = 25;
	[Header("Sound Configuration")]
	public AudioSource audioSource;
	public AudioClip pointIncreaseSound;
	public float pointIncreaseVolume;
	public AudioClip pointDecreaseSound;
	public float pointDecreaseVolume;
	public AudioClip jailCloseSound;
	public float jailCloseVolume;
	public AudioClip jailOpenSound;
	public float jailOpenVolume;
	public AudioClip roundFinishSound;
	public float roundFinishVolume;
	public AudioClip allTieSound;
	public float allTieVolume;


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

	public static void ResetScores() {
		scores.Clear();
		incrementingScores.Clear();
		manager.winnerText.text = "";
	}

	public static void SetupScores(int playerCount) {
		GameObject scoreSource = manager.prefabSource.Find("PlayerScore").gameObject;
		Transform timer = manager.roundPanel.Find("Timer");
		float timerWidth = timer.GetComponent<RectTransform>().sizeDelta.x;

		for (int i = 0; i < playerCount; i++) {
			int side = Mathf.FloorToInt((float) i / playerCount + 0.5f) == 0 ? -1 : 1;
			int offset = Mathf.FloorToInt(Mathf.Abs(playerCount / 2f - i) - (side == -1 ? 0.5f : 0));
			scores.Add((Player) i, 0);
			incrementingScores.Add((Player) i, new List<ScoreIncrementingCoroutine>());

			// Add the score UI
			Transform playerContainer = (Instantiate(scoreSource, manager.roundPanel) as GameObject).transform;
			playerContainer.name = "Player" + i;
			playerContainer.transform.SetParent(manager.roundPanel, false);
			RectTransform rect = playerContainer.GetComponent<RectTransform>();
			// Set the position
			Vector2 position = rect.anchoredPosition;
			position.x = (timerWidth / 2 + (manager.padding * (1 + offset)) + (rect.sizeDelta.x * offset)) * side;
			rect.anchoredPosition = position;
			// Set the pivot point
			Vector2 pivot = rect.pivot;
			pivot.x = side == -1 ? 1 : 0;
			rect.pivot = pivot;
			// Color ronaldo
			Transform ronaldoUI = rect.Find("Ronaldo");
			ronaldoUI.Find("Body").gameObject.GetComponent<Image>().color = PlayerColor.Get(i);
			ronaldoUI.Find("Arms Free").gameObject.GetComponent<Image>().color = PlayerColor.Get(i);
			ronaldoUI.Find("Arms Jailed").gameObject.GetComponent<Image>().color = PlayerColor.Get(i);
		}
	}

	public static void ChangeScoreBy(Player player, int score) {
		scores[player] += score;

		if (scores[player] < 0) {
			scores[player] = 0;
		}

		if (score > 0) {
			
		} else if (score < 0) {
			manager.audioSource.PlayOneShot(manager.pointDecreaseSound, manager.pointDecreaseVolume);
		}

		Transform text = manager.roundPanel.Find("Player" + (int) player).Find("Panel").Find("Score");
		text.GetComponent<Text>().text = scores[player].ToString();
	}

	public static void StartIncreasingScoreBy(Player player, int score) {
		incrementingScores[player].Add(new ScoreIncrementingCoroutine(manager.StartCoroutine(IncrementScoreBy(player, score)), score));
	}

	public static void Jail(Player captive, Player captor) {
		Transform ronaldoUI = manager.roundPanel.Find("Player" + (int) captive).Find("Ronaldo");
		ronaldoUI.Find("Arms Free").gameObject.SetActive(false);
		ronaldoUI.Find("Bars").gameObject.SetActive(true);
		ronaldoUI.Find("Bars").GetComponent<Image>().color = PlayerColor.Get((int) captor);
		ronaldoUI.Find("Arms Jailed").gameObject.SetActive(true);
		manager.audioSource.PlayOneShot(manager.jailCloseSound, manager.jailCloseVolume);
		//manager.audioSource.PlayOneShot(manager.pointIncreaseSound, manager.pointIncreaseVolume);
	}

	public static void Release(Player player) {
		Transform ronaldoUI = manager.roundPanel.Find("Player" + (int) player).Find("Ronaldo");
		ronaldoUI.Find("Arms Free").gameObject.SetActive(true);
		ronaldoUI.Find("Bars").gameObject.SetActive(false);
		ronaldoUI.Find("Arms Jailed").gameObject.SetActive(false);
		manager.audioSource.PlayOneShot(manager.jailOpenSound, manager.jailOpenVolume);
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
			manager.audioSource.PlayOneShot(manager.allTieSound, manager.allTieVolume);
		} else {
			manager.winnerText.text = "Player " + (((int) players[0]) + 1) + " wins!";

			manager.audioSource.PlayOneShot(manager.roundFinishSound, manager.roundFinishVolume);
		}
	}
}
