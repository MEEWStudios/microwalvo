using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	[Header("Model Configuration")]
	public Transform prefabSource;
	[Header("NPCs")]
	public int fakeRonaldoCount;
	public int personCount;
	public int spawnDistanceFromSpotlight = 10;
	[Header("Items")]
	public int itemCount = 10;
	public GameObject itemAnimation;
	[Header("Rounds")]
	public int roundTime = 120;
	[Header("User Interface")]
	public Text timerText;
	[Header("Camera Blur Pane")]
	public DrawOnTexture draw;

	public static GameManager manager;
	private bool roundInProgress = false;
	private float currentRoundTime;
	private static Dictionary<Player, Transform> playerMap = new Dictionary<Player, Transform>();
	private static List<GameObject> spotlightColliders = new List<GameObject>();
	// Dictionary<Player captive, Player captor>
	private static Dictionary<Player, Player> captures = new Dictionary<Player, Player>();
	private Transform map;
	private Transform players;
	private Transform npcs;
	private Transform items;

	// Awake is called when the script instance is being loaded
	void Awake() {
		manager = this;
		map = GameObject.Find("Map").transform;
		players = new GameObject("Players").transform;
		players.parent = map;
		npcs = new GameObject("NPCs").transform;
		npcs.parent = map;
		items = new GameObject("Items").transform;
		items.parent = map;
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
					ResetRound();
					ScoreManager.ResetScores();
					StartRound();
				}
			} else if (Input.GetKeyDown(KeyCode.Space)) {
				ResetRound();
				ScoreManager.ResetScores();
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
		Transform characterSource = prefabSource.Find("Characters");
		GameObject sourcePerson = characterSource.Find("Person").gameObject;
		GameObject sourceLookAlike = characterSource.Find("Fake Ronaldo").gameObject;
		GameObject sourceSpotlight = prefabSource.Find("Spotlight").gameObject;
		GameObject sourceRonaldo = characterSource.Find("Ronaldo").gameObject;
		Transform shirtSource = prefabSource.Find("Clothing").Find("Shirts");
		Transform accessoriesSource = prefabSource.Find("Accessories");
		Transform itemSource = prefabSource.Find("Items");

		currentRoundTime = 0;

		for (int i = 0; i < playerCount; i++) {
			// Create player object
			Transform player = new GameObject("Player" + i).transform;
			player.parent = players;
			// Attach player data
			PlayerData data = player.gameObject.AddComponent<PlayerData>();
			data.player = (Player) i;
			// Save a reference to the player object
			playerMap.Add((Player) i, player);
			// Add player's spotlight
			int x = i % 2 == 1 ? -20 : 20;
			int z = i / 2 == 0 ? 0 : -30;
			GameObject newSpotlight = Instantiate(sourceSpotlight, new Vector3(x, 0, z), Quaternion.identity, player) as GameObject;
			newSpotlight.name = "Spotlight";
			// Color spotlight ring
			newSpotlight.transform.Find("SpotlightCollider").Find("Canvas").Find("Image").gameObject.GetComponent<Image>().color = PlayerColor.Get(i);
			// Add SpotlightControl
			SpotlightControl control = newSpotlight.AddComponent<SpotlightControl>();
			// Add Player specifier
			control.player = (Player) i;
			// Tell the detection script about the controller
			newSpotlight.transform.Find("SpotlightCollider").GetComponent<Detection>().control = control;
			// Add this spotlight to the list of colliders to avoid spawning Ronaldo or his look alikes under it
			spotlightColliders.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			// Add keyboard control
			if (i == 0) {
				KeyboardControl keyControl = newSpotlight.AddComponent<KeyboardControl>();
			}

			// Add the spotlight to the texture draw script
			draw.spotlights.Add(newSpotlight.transform.Find("SpotlightCollider").gameObject);

			// Spawn player's Ronaldo
			GameObject ronaldo = SpawnNPC(sourceRonaldo, GetRandomPointOnMap(sourceRonaldo.transform.position.y, new Vector3(spawnDistanceFromSpotlight, 10, spawnDistanceFromSpotlight)), Quaternion.identity, player);
			ronaldo.name = "Ronaldo";
			// Tag as a Ronaldo
			ronaldo.tag = "Real Ronaldo";
			// Set color
			ronaldo.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[0].color = PlayerColor.Get(i);
			ronaldo.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[1].color = PlayerColor.Get(i) * new Color(0.5f, 0.5f, 0.5f);
			ronaldo.transform.Find("sweater2").Find("pCylinder1").GetComponent<SkinnedMeshRenderer>().material.color = PlayerColor.Get(i);
			// Add Player specifier
			NPCTraits traits = ronaldo.AddComponent<NPCTraits>();
			traits.player = (Player) i;

			// Spawn look alikes
			for (int j = 0; j < fakeRonaldoCount; j++) {
				// Spawn look alike
				GameObject npc = SpawnNPC(sourceLookAlike, GetRandomPointOnMap(sourceLookAlike.transform.position.y, new Vector3(spawnDistanceFromSpotlight, 10, spawnDistanceFromSpotlight)), Quaternion.identity, npcs);
				// Tag as a look alike
				npc.tag = "Fake Ronaldo";
				// Set skin color
				npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[0].color = SkinColor.GetRandom();
				npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[1].color = PlayerColor.Get(i) * new Color(0.5f, 0.5f, 0.5f);
				npc.transform.Find("sweater2").Find("pCylinder1").GetComponent<SkinnedMeshRenderer>().material.color = PlayerColor.Get(i);
			}
		}

		// Spawn NPCs
		// Spawn people
		for (int i = 0; i < personCount; i++) {
			// Spawn person
			GameObject npc = SpawnNPC(sourcePerson, GetRandomPointOnMap(sourcePerson.transform.position.y), Quaternion.identity, npcs);
			// Set skin color
			npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[0].color = SkinColor.GetRandom();
			npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[1].color = PantColor.GetRandom();
			// Pick a random shirt
			Transform clothing = (Instantiate(shirtSource.GetChild(Random.Range(0, shirtSource.childCount)).gameObject, npc.transform) as GameObject).transform;
			clothing.transform.localPosition = new Vector3(0, 0, 0);
			// Add the Wearable script
			clothing.transform.Find("pCylinder1").gameObject.AddComponent<Wearable>();
			// Pick a random shirt color
			Color color = ShirtColor.GetRandom();
			Material[] materials = clothing.Find("pCylinder1").GetComponent<SkinnedMeshRenderer>().materials;
			for (int j = 0; j < materials.Length; j++) {
				// Apply color
				materials[j].color = color;
			}
			// Choose random accessories
			foreach (Transform accessoryType in accessoriesSource) {
				int selection = Random.Range(-1, accessoryType.childCount);

				if (selection == -1) {
					continue;
				}

				Transform accessoryBody = accessoryType.GetChild(selection);
				Transform[] accessoryList = accessoryBody.GetComponent<AccessoryLocation>().accessories;
				Transform accessorySource = accessoryList[Random.Range(0, accessoryList.Length)];
				Transform temporary = accessorySource.parent;
				Stack<string> path = new Stack<string>();

				// Find the path that must be traversed to place the accessory on the right bone
				while (temporary != accessoryBody) {
					if (temporary.parent == null) {
						return;
					}

					path.Push(temporary.name);
					temporary = temporary.parent;
				}

				// Traverse through the body to get the correct bone
				temporary = npc.transform;
				while (path.Count > 0) {
					temporary = temporary.Find(path.Pop());
				}

				// Spawn the accessory and set it's position, rotation, and scale
				GameObject accessory = Instantiate(accessorySource.gameObject, temporary) as GameObject;
				accessory.transform.localPosition = accessorySource.localPosition;
				accessory.transform.rotation = accessorySource.rotation;
				accessory.transform.localScale = accessorySource.localScale;
			}
		}

		// Spawn items
		for (int i = 0; i < itemCount; i++) {
			// Pick a random item
			GameObject source = itemSource.GetChild(Random.Range(0, itemSource.childCount)).gameObject;
			// Spawn it
			SpawnItem(source);
		}

		roundInProgress = true;
		ScoreManager.SetupScores(playerCount);
	}

	void EndRound() {
		// Disable player controls
		ControlManager.UnregisterControls(typeof(SpotlightControl));
		ControlManager.UnregisterControls(typeof(KeyboardControl));

		// Stop all NPCs
		foreach (Transform child in npcs) {
			StopNPC(child.gameObject);
		}
		foreach (Transform player in players) {
			foreach (Transform transform in player.transform) {
				if (transform.tag == "Real Ronaldo") {
					StopNPC(transform.gameObject);
				}
			}
		}

		ScoreManager.DisplayResults();

		//disable all spotlight mesh colliders
		foreach (Transform player in players) {
			player.Find("Spotlight").transform.Find("SpotlightCollider").GetComponent<MeshCollider>().enabled = false;
		}

	}

	void ResetRound() {
		playerMap.Clear();
		spotlightColliders.Clear();
		captures.Clear();

		foreach (Transform child in players) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in npcs) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in items) {
			Destroy(child.gameObject);
		}
	}

	public static void CaptureRonaldo(GameObject ronaldo, Player captor) {
		Player captive = ronaldo.GetComponent<NPCTraits>().player;

		// Keep track of which player captured which player's ronaldo
		captures.Add(captive, captor);

		// Turn off Ronaldo when found
		ronaldo.SetActive(false);
		ronaldo.transform.position = GetRandomPointOnMap(ronaldo.transform.position.y, new Vector3(manager.spawnDistanceFromSpotlight, 10, manager.spawnDistanceFromSpotlight));
		// Add points to the corresponding spotlight's score over time
		ScoreManager.StartIncreasingScoreBy(captor, 1);
		ScoreManager.Jail(captive, captor);

		// Spawn Key after a delay
		manager.StartCoroutine(spawnKey(5.0f));
	}

	public static void ReleaseRonaldo(Player player) {
		Transform playerGroup = GetPlayerGroup(player);

		// Stop increasing the captor's score
		ScoreManager.StopIncreasingScoreBy(captures[player], 1);
		ScoreManager.Release(player);
		captures.Remove(player);
		// Enable the released ronaldo
		playerGroup.Find("Ronaldo").gameObject.SetActive(true);
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

	GameObject SpawnItem(GameObject original) {
		// Get a random position
		Vector3 position = GameManager.GetRandomPointOnMap(original.transform.position.y);
		// Spawn the item
		GameObject item = Instantiate(original, position, original.transform.rotation, items) as GameObject;
		// Tag as an item
		item.tag = "Item";
		// Spawn the animation effect
		GameObject sparkle = Instantiate(itemAnimation, position, Quaternion.identity, item.transform) as GameObject;
		sparkle.transform.localPosition = new Vector3(0, 0.5f, 0.5f);
		// Set the animation size
		sparkle.transform.localScale = new Vector3(4, 4, 4);

		return item;
	}

	void StopNPC(GameObject npc) {
		npc.GetComponent<NavMeshAgent>().enabled = false;
		npc.GetComponent<NPCController>().enabled = false;
		npc.GetComponent<Animator>().SetInteger("moveState", 0);
	}

	public static Vector3 GetRandomPointOnMap(float yPosition) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = new Vector3(Random.Range(-119f, 119f), 0, Random.Range(-53f, 53f));
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return new Vector3(hit.position.x, yPosition, hit.position.z);
	}

	public static Vector3 GetRandomPointOnMap(float yPosition, Vector3 size) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = new Vector3(Random.Range(-119f, 119f), 0, Random.Range(-53f, 53f));
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

	public static Transform GetPlayerGroup(Player player) {
		return playerMap[player];
	}

	public static IEnumerator spawnKey(float delay) {
		yield return new WaitForSeconds(delay);

		manager.SpawnItem(manager.prefabSource.Find("key").gameObject);

		Debug.Log("Key spawned");
	}
}
