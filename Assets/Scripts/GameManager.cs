using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {
	public GameObject prefabSource;
	public int roundTime = 120;
	public Text timerText;
	public DrawOnTexture draw;

	private bool roundInProgress = false;
	private float currentRoundTime = 0;
	private ScoreManager scoreManager;

	// Awake is called when the script instance is being loaded
	void Awake() {
		scoreManager = this.GetComponent<ScoreManager>();
	}

	// Use this for initialization
	void Start() {
		//MainMenu();
		StartRound();
	}

	// Update is called once per frame
	void Update() {
		if (roundInProgress) {
			currentRoundTime += Time.deltaTime;
			if ((roundTime - currentRoundTime) > 0) {
				timerText.text = TimestampToString(roundTime - currentRoundTime);
			} else {
				roundInProgress = false;
				EndRound();
			}
		}
	}

	void MainMenu() {
		ControlManager.RegisterGlobalControl(new MainMenu());
	}

	void StartRound() {
		int playerCount = EZGM.EZGamepadCount();
		GameObject sourceSpotlight = prefabSource.transform.Find("Spotlight").gameObject;
		GameObject sourceRonaldo = prefabSource.transform.Find("Characters").Find("Ronaldo").gameObject;
		Transform map = GameObject.Find("Map").transform;

		for (int i = 0; i < playerCount; i++) {
			int x = i % 2 == 1 ? -20 : 20;
			int z = i / 2 == 0 ? 0 : -30;
			GameObject newSpotlight = Instantiate(sourceSpotlight, new Vector3(x, 0 , z), Quaternion.identity, map) as GameObject;
			SpotlightControl control = newSpotlight.AddComponent<SpotlightControl>();
			control.player = (Player) i;
			newSpotlight.transform.Find("SpotlightCollider").GetComponent<detection>().control = control;

			// Add the spotlight to the texture draw script
			draw.spotlights.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			GameObject ronaldo = Instantiate(sourceRonaldo, new Vector3(Random.Range(-80, 80), sourceRonaldo.transform.position.y, Random.Range(-80, 80)), Quaternion.identity, map) as GameObject;
			ronaldo.AddComponent<NPCController>();
			NPCTraits traits = ronaldo.AddComponent<NPCTraits>();
			traits.player = (Player) i;
		}

		roundInProgress = true;
		//scoreManager.ResetScores(EZGM.EZGamepadCount());
		scoreManager.ResetScores(4);
	}

	void EndRound() {
		timerText.text = "Time Up!";
		scoreManager.DisplayResults();
		ControlManager.UnregisterControls(typeof(SpotlightControl));
	}

	string TimestampToString(float timestamp) {
		int minutes = Mathf.FloorToInt(timestamp / 60);
		timestamp -= minutes * 60;
		int seconds = Mathf.FloorToInt(timestamp);

		return minutes + ":" + (seconds < 10 ? "0" + seconds : "" + seconds);
	}
}
