using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	[Header("Model Configuration")]
	public GameObject prefabSource;
	[Header("NPCs")]
	public int fakeRonaldoCount;
	public int personCount;
	public int spawnRepel = 10;
	[Header("Rounds")]
	public int roundTime = 120;
	[Header("User Interface")]
	public Text timerText;
	[Header("Camera Blur Pane")]
	public DrawOnTexture draw;

	private bool roundInProgress = false;
	private float currentRoundTime = 0;
	private ScoreManager scoreManager;
	private static List<GameObject> spotlightColliders = new List<GameObject>();

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
			// Press Space (Keyboard), A (Xbox), or X (PS) to start the game
			if (EZGM.EZGamepadCount() > 0) {
				if (EZGM.GetEZGamepad((Player) 0).buttonSouth.justPressed) {
					StartRound();
				}
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				StartRound();
			}
		} else {
			currentRoundTime += Time.deltaTime;
			if ((roundTime - currentRoundTime) > 0) {
				timerText.text = TimestampToString(roundTime - currentRoundTime);
			} else {
				timerText.text = TimestampToString(0);
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
		GameObject sourceLookAlike = characterSource.Find("Fake Ronaldo").gameObject;
		GameObject sourceSpotlight = prefabSource.transform.Find("Spotlight").gameObject;
		GameObject sourceRonaldo = characterSource.Find("Ronaldo").gameObject;
		Transform map = GameObject.Find("Map").transform;
		GameObject npcs = new GameObject("NPCs");
		npcs.transform.parent = map;

		for (int i = 0; i < playerCount; i++) {
			// Add player's spotlight
			int x = i % 2 == 1 ? -20 : 20;
			int z = i / 2 == 0 ? 0 : -30;
			GameObject newSpotlight = Instantiate(sourceSpotlight, new Vector3(x, 0, z), Quaternion.identity, map) as GameObject;
			// Add SpotlightControl
			SpotlightControl control = newSpotlight.AddComponent<SpotlightControl>();
			// Add Player specifier
			control.player = (Player) i;
			// Tell the detection script about the controller
			newSpotlight.transform.Find("SpotlightCollider").GetComponent<detection>().control = control;
			// Add this spotlight to the list of colliders to avoid spawning Ronaldo or his look alikes under it
			spotlightColliders.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			// Add keyboard control
			if (i == 0) {
				KeyboardControl keyControl = newSpotlight.AddComponent<KeyboardControl>();
			}

			// Add the spotlight to the texture draw script
			draw.spotlights.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			// Spawn player's Ronaldo
			GameObject ronaldo = SpawnNPC(sourceRonaldo, GetRandomPointOnMap(sourceRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel)), Quaternion.identity, npcs.transform);
			// Tag as a Ronaldo
			ronaldo.tag = "Real Ronaldo";
			// Add Player specifier
			NPCTraits traits = ronaldo.AddComponent<NPCTraits>();
			traits.player = (Player) i;
		}

		// Spawn NPCs
		// Spawn people
		for (int i = 0; i < personCount; i++) {
			// Spawn person
			GameObject npc = SpawnNPC(sourcePerson, GetRandomPointOnMap(sourcePerson.transform.position.y), Quaternion.identity, npcs.transform);
			// Set skin color
			npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().material.color = SkinColor.GetRandom();
		}

		// Spawn look alikes
		for (int i = 0; i < fakeRonaldoCount; i++) {
			// Spawn look alike
			GameObject npc = SpawnNPC(sourceLookAlike, GetRandomPointOnMap(sourceLookAlike.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel)), Quaternion.identity, npcs.transform);
			// Tag as a look alike
			npc.tag = "Fake Ronaldo";
		}

		roundInProgress = true;
		//scoreManager.ResetScores(EZGM.EZGamepadCount());
		scoreManager.ResetScores(4);
	}

	void EndRound() {
		scoreManager.DisplayResults();
		ControlManager.UnregisterControls(typeof(SpotlightControl));
	}

	string TimestampToString(float timestamp) {
		timestamp += 1;
		int minutes = Mathf.FloorToInt(timestamp / 60);
		timestamp -= minutes * 60;
		int seconds = Mathf.CeilToInt(timestamp) - 1;

		return minutes + ":" + (seconds < 10 ? "0" + seconds : "" + seconds);
	}

	GameObject SpawnNPC(GameObject original, Vector3 position, Quaternion rotation, Transform parent) {
		GameObject npc = Instantiate(original, position, rotation, parent) as GameObject;
		// Add NPCController
		npc.AddComponent<NPCController>();
		// Enable the Nav Mesh Agent
		npc.GetComponent<NavMeshAgent>().enabled = true;
		// Enable the Animator
		npc.GetComponent<Animator>().enabled = true;

		return npc;
	}

	public static Vector3 GetRandomPointOnMap(float yPosition) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = new Vector3(Random.Range(-200, 200), 0, Random.Range(-80, 80));
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return new Vector3(hit.position.x, yPosition, hit.position.z);
	}

	public static Vector3 GetRandomPointOnMap(float yPosition, Vector3 size) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = new Vector3(Random.Range(-200, 200), 0, Random.Range(-80, 80));
		} while (CollidesWithSpotlights(randomPoint, size) || !NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return new Vector3(hit.position.x, yPosition, hit.position.z);
	}

	public static bool CollidesWithSpotlights(Vector3 position, Vector3 size) {
		foreach (GameObject spotlight in spotlightColliders) {
			Vector3 spotlightPosition = spotlight.transform.position;
			Vector3 spotlightSize = spotlight.GetComponent<MeshCollider>().bounds.size;

			if ((Mathf.Abs(spotlightPosition.x - position.x) * 2 < (spotlightSize.x + size.x)) && (Mathf.Abs(spotlightPosition.z - position.z) * 2 < (spotlightSize.z + size.z))) {
				return true;
			}
		}

		return false;
	}
}
