using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {
	[Header("Model Configuration")]
	public GameObject prefabSource;
	[Header("NPCs")]
	public int fakeRonaldoCount;
	public int personCount;
	[Header("Rounds")]
	public int roundTime = 120;
	[Header("User Interface")]
	public Text timerText;
	[Header("Camera Blur Pane")]
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
		//StartRound();
	}

	// Update is called once per frame
	void Update() {
		if (!roundInProgress) {
			if (EZGM.EZGamepadCount() > 0) {
				if (EZGM.GetEZGamepad((Player) 0).buttonSouth.justPressed) {
					StartRound();
				}
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				StartRound();
			}
		} else if (roundInProgress) {
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
		int playerCount = EZGM.EZGamepadCount() == 0 ? 1 : EZGM.EZGamepadCount();
		Transform characterSource = prefabSource.transform.Find("Characters");
		GameObject sourcePerson = characterSource.Find("Person").gameObject;
		GameObject sourceRelative = characterSource.Find("Fake Ronaldo").gameObject;
		GameObject sourceSpotlight = prefabSource.transform.Find("Spotlight").gameObject;
		GameObject sourceRonaldo = characterSource.Find("Ronaldo").gameObject;
		Transform characters = GameObject.Find("Map").transform.Find("Characters");

		// Spawn NPCs
		// Spawn people
		for (int i = 0; i < personCount; i++) {
			// Spawn person
			GameObject npc = Instantiate(sourcePerson, GetRandomPointOnMap(sourcePerson.transform.position.y), Quaternion.identity, characters) as GameObject;
			// Add NPCController
			npc.AddComponent<NPCController>();
			// Enable the Nav Mesh Agent
			npc.GetComponent<NavMeshAgent>().enabled = true;
			// Enable gravity
			npc.GetComponent<Rigidbody>().useGravity = true;
		}

		// Spawn relatives
		for (int i = 0; i < fakeRonaldoCount; i++) {
			// Spawn relative
			GameObject npc = Instantiate(sourceRelative, GetRandomPointOnMap(sourceRelative.transform.position.y), Quaternion.identity, characters) as GameObject;
			// Add NPCController
			npc.AddComponent<NPCController>();
			// Enable the Nav Mesh Agent
			npc.GetComponent<NavMeshAgent>().enabled = true;
			// Enable gravity
			npc.GetComponent<Rigidbody>().useGravity = true;
			// Tag as a relative
			npc.tag = "Fake Ronaldo";
		}

		for (int i = 0; i < playerCount; i++) {
			// Add player's spotlight
			int x = i % 2 == 1 ? -20 : 20;
			int z = i / 2 == 0 ? 0 : -30;
			GameObject newSpotlight = Instantiate(sourceSpotlight, new Vector3(x, 0, z), Quaternion.identity, characters) as GameObject;
			// Add SpotlightControl
			SpotlightControl control = newSpotlight.AddComponent<SpotlightControl>();
			// Add Player specifier
			control.player = (Player) i;
			// Tell the detection script about the controller
			newSpotlight.transform.Find("SpotlightCollider").GetComponent<detection>().control = control;

			// Add keyboard control
			if (i == 0) {
				KeyboardControl keyControl = newSpotlight.AddComponent<KeyboardControl>();
				keyControl.speed = 50;
			}

			// Add the spotlight to the texture draw script
			draw.spotlights.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			// Spawn player's Ronaldo
			GameObject ronaldo = Instantiate(sourceRonaldo, GetRandomPointOnMap(sourceRonaldo.transform.position.y), Quaternion.identity, characters) as GameObject;
			// Add NPCController
			ronaldo.AddComponent<NPCController>();
			// Enable the Nav Mesh Agent
			ronaldo.GetComponent<NavMeshAgent>().enabled = true;
			// Enable gravity
			ronaldo.GetComponent<Rigidbody>().useGravity = true;
			// Add Player specifier
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

	Vector3 GetRandomPointOnMap(float yPosition) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = new Vector3(Random.Range(-200, 200), 0, Random.Range(-80, 80));
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return new Vector3(hit.position.x, yPosition, hit.position.z);
	}
}
