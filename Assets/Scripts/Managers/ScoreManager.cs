using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {
	private class ScoreIncrementer {
		public Player target;
		public Player source;
		public int score;
		public float interval;
		public float timer;

		public ScoreIncrementer(Player target, Player source, int score, float interval) {
			this.target = target;
			this.source = source;
			this.score = score;
			this.interval = interval;
			timer = interval;
		}

		public void Run(float deltaTime) {
			timer -= deltaTime;

			if (timer <= 0) {
				timer = interval;
				ChangeScoreBy(target, score);
			}
		}
	}

	[Header("UI")]
	public Transform prefabSource;
	public Transform roundPanel;
	public Text winnerText;
	public int padding = 25;
	[Header("Audio")]
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
	public static GameObject overlay;
	private static GameObject endScreen;
	private static GameObject tieScreen;
    

	private static ScoreManager manager;
	private static readonly Dictionary<Player, int> scores = new Dictionary<Player, int>();
	private static readonly Dictionary<Player, List<ScoreIncrementer>> incrementingScores = new Dictionary<Player, List<ScoreIncrementer>>();

	// Start is called before the first frame update
	void Start() {
		manager = this;
		overlay = GameObject.Find("Overlay");
		endScreen = overlay.transform.Find("End Game Screen").gameObject;
		tieScreen = overlay.transform.Find("Tie Screen").gameObject;
	}

	// UpdateScores is called once per frame
	public static void UpdateScores() {
		float deltaTime = Time.deltaTime;

		foreach (KeyValuePair<Player, List<ScoreIncrementer>> incrementers in incrementingScores) {
			foreach (ScoreIncrementer incrementer in incrementers.Value) {
				incrementer.Run(deltaTime);
			}
		}
	}

	public static void ResetScores() {
		for (int i = 0; i < scores.Count; i++) {
			Destroy(manager.roundPanel.Find("Player" + i).gameObject);
		}

		scores.Clear();
		incrementingScores.Clear();
		manager.winnerText.text = "";
		tieScreen.SetActive(false);
		endScreen.SetActive(false);
	}

	public static void SetupScores(int playerCount) {
		GameObject scoreSource = manager.prefabSource.Find("PlayerScore").gameObject;
		Transform timer = manager.roundPanel.Find("TimerPanel");
		float timerWidth = timer.GetComponent<RectTransform>().sizeDelta.x;

		for (int i = 0; i < playerCount; i++) {
			int side = Mathf.FloorToInt((float) i / playerCount + 0.5f) == 0 ? -1 : 1;
			int offset = Mathf.FloorToInt(Mathf.Abs(playerCount / 2f - i) - (side == -1 ? 0.5f : 0));
			scores.Add((Player) i, 0);
			incrementingScores.Add((Player) i, new List<ScoreIncrementer>());

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
			ronaldoUI.Find("Body").GetComponent<Image>().color = PlayerColor.Get(i);
			ronaldoUI.Find("Arms Free").GetComponent<Image>().color = PlayerColor.Get(i);
			ronaldoUI.Find("Arms Jailed").GetComponent<Image>().color = PlayerColor.Get(i);
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

	public static void StartIncreasingScoreBy(Player captor, Player captive, int score) {
		//incrementingScores[player].Add(new ScoreIncrementer(manager.StartCoroutine(IncrementScoreBy(player, score)), score));
		incrementingScores[captor].Add(new ScoreIncrementer(captor, captive, score, 0.5f));
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

	public static void StopIncreasingScoreBy(Player captor, Player captive) {
		List<ScoreIncrementer> routines = incrementingScores[captor];

		foreach (ScoreIncrementer incrementer in routines) {
			if (incrementer.source == captive) {
				routines.Remove(incrementer);

				return;
			}
		}
	}

	public static void DisplayResults() {
		int maxScore = 0;
		List<Player> players = new List<Player>();
		Sprite redWinSprite = Resources.Load<Sprite>("end_screens/red_win");
		Sprite purpleWinSprite = Resources.Load<Sprite>("end_screens/purple_win");
		Sprite blueWinSprite = Resources.Load<Sprite>("end_screens/blue_win");
		Sprite greenWinSprite = Resources.Load<Sprite>("end_screens/green_win");

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

			//turn off the corresponding sprite if they weren't part of the tie
			if(!(manager.winnerText.text.Contains("1")) && manager.winnerText.text != "All players have tied!") {
				tieScreen.transform.Find("Red Separate").gameObject.SetActive(false);
			}
			if(!(manager.winnerText.text.Contains("2")) && manager.winnerText.text != "All players have tied!") {
				tieScreen.transform.Find("Purple Separate").gameObject.SetActive(false);
			}
			if(!(manager.winnerText.text.Contains("3")) && manager.winnerText.text != "All players have tied!") {
				tieScreen.transform.Find("Blue Separate").gameObject.SetActive(false);
			}
			if(!(manager.winnerText.text.Contains("4")) && manager.winnerText.text != "All players have tied!") {
				tieScreen.transform.Find("Green Separate").gameObject.SetActive(false);
			}

			tieScreen.SetActive(true);


		} else {
			manager.winnerText.text = "Player " + (((int) players[0]) + 1) + " wins!";

			//Individual player win screens
			if(manager.winnerText.text == "Player 1 wins!") {
				endScreen.GetComponent<Image>().sprite = redWinSprite;
				endScreen.SetActive(true);
			} 
			else if(manager.winnerText.text == "Player 2 wins!") {
				endScreen.GetComponent<Image>().sprite = purpleWinSprite;
				endScreen.SetActive(true);
			} 
			else if(manager.winnerText.text == "Player 3 wins!") {
				endScreen.GetComponent<Image>().sprite = blueWinSprite;
				endScreen.SetActive(true);
			} 
			else if(manager.winnerText.text == "Player 4 wins!") {
				endScreen.GetComponent<Image>().sprite = greenWinSprite;
				endScreen.SetActive(true);
			} 

			manager.audioSource.PlayOneShot(manager.roundFinishSound, manager.roundFinishVolume);
		}
	}
}
