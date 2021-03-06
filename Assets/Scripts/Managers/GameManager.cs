﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	[Header("Prefabs")]
	public Transform prefabSource;
	[Header("User Interface")]
	public Transform overlay;
	public Text timerText;
	[Header("Audio")]
	public AudioSource menuSource;
	public AudioSource[] ambientSources;
	[Header("NPCs")]
	public int fakeRonaldoCount;
	public int personCount;
	public int spawnDistanceFromSpotlight = 10;
	[Header("Items")]
	public int itemCount = 10;
	public GameObject itemAnimation;
	[Header("Rounds")]
	public int roundTime = 120;

	public static GameManager manager;
	private static bool roundInProgress = false;
	private static bool roundIsPaused = false;
	private static float currentRoundTime;
	private static readonly Dictionary<Player, Transform> playerMap = new Dictionary<Player, Transform>();
	private static readonly List<NPCController> controllers = new List<NPCController>();
	private static readonly List<Transform> spotlightColliders = new List<Transform>();
	// Dictionary<Player captive, Player captor>
	private static readonly Dictionary<Player, Player> captorOf = new Dictionary<Player, Player>();
	private static Transform map;
	private static Transform players;
	private static Transform npcs;
	private static Transform items;

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
		ControlManager.RegisterGlobalControl(MenuController.instance);
		MenuController.SetTopLevelMenu(overlay.Find("MainMenu").Find("Panel"));
		// Play menu music
		menuSource.Play();
	}

	// Update is called once per frame
	void Update() {
		if (roundInProgress && !roundIsPaused) {
			currentRoundTime += Time.deltaTime;

			// Update scores
			ScoreManager.UpdateScores();
			// Update agents
			foreach (Transform npc in npcs) {
				npc.GetComponent<NPCController>().UpdateAgent();
			}
			foreach (KeyValuePair<Player, Transform> pair in playerMap) {
				if (!captorOf.ContainsKey(pair.Key)) {
					pair.Value.Find("Ronaldo").GetComponent<NPCController>().UpdateAgent();
				}
			}
			// Update colliders
			foreach (Transform colliderObject in spotlightColliders) {
				colliderObject.GetComponent<Detection>().UpdateCollisions();
			}

			// Update round timer
			if ((roundTime - currentRoundTime) > 0) {
				timerText.text = TimestampToString(roundTime - currentRoundTime);
			} else {
				timerText.text = TimestampToString(0);
				roundInProgress = false;
				EndRound();
			}
		}
	}

	public static void StartRound() {
		int playerCount = EZGM.EZGamepadCount() == 0 ? 1 : EZGM.EZGamepadCount();
		Transform characterSource = manager.prefabSource.Find("Characters");
		GameObject sourcePerson = characterSource.Find("Person").gameObject;
		GameObject sourceLookAlike = characterSource.Find("Fake Ronaldo").gameObject;
		GameObject sourceSpotlight = manager.prefabSource.Find("Spotlight").gameObject;
		GameObject sourceRonaldo = characterSource.Find("Ronaldo").gameObject;
		Transform shirtSource = manager.prefabSource.Find("Clothing").Find("Shirts");
		Transform accessoriesSource = manager.prefabSource.Find("Accessories");
		Transform itemSource = manager.prefabSource.Find("Items");

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
			newSpotlight.transform.Find("SpotlightCollider").Find("Canvas").Find("Image").GetComponent<Image>().color = PlayerColor.Get(i);
			// Add SpotlightControl
			SpotlightControl control = newSpotlight.AddComponent<SpotlightControl>();
			// Add Player specifier
			control.player = (Player) i;
			// Register the control
			ControlManager.RegisterControl(control, (Player) i);
			// Get Detection script
			Detection detection = newSpotlight.transform.Find("SpotlightCollider").GetComponent<Detection>();
			// Enable the detection script
			detection.enabled = true;
			// Add Player specifier
			detection.player = (Player) i;
			// Add this spotlight to the list of colliders to avoid spawning Ronaldo or his look alikes under it
			spotlightColliders.Add(newSpotlight.transform.Find("SpotlightCollider"));

			// Add keyboard control
			if (i == 0 && EZGM.EZGamepadCount() == 0) {
				newSpotlight.AddComponent<KeyboardControl>();
			}

			// Spawn player's Ronaldo
			GameObject ronaldo = SpawnNPC(sourceRonaldo, GetRandomPointOnMap(sourceRonaldo.transform.position.y, new Vector3(manager.spawnDistanceFromSpotlight, 10, manager.spawnDistanceFromSpotlight)), Quaternion.identity, player);
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
			// Setup the progress indicator
			ronaldo.GetComponent<ProgressIndicator>().Initialize(manager.prefabSource.Find("SpotlightProgress").gameObject);

			// Spawn look alikes
			for (int j = 0; j < manager.fakeRonaldoCount; j++) {
				// Spawn look alike
				GameObject npc = SpawnNPC(sourceLookAlike, GetRandomPointOnMap(sourceLookAlike.transform.position.y, new Vector3(manager.spawnDistanceFromSpotlight, 10, manager.spawnDistanceFromSpotlight)), Quaternion.identity, npcs);
				// Tag as a look alike
				npc.tag = "Fake Ronaldo";
				// Set skin color
				npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[0].color = SkinColor.GetRandom();
				npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[1].color = PlayerColor.Get(i) * new Color(0.5f, 0.5f, 0.5f);
				npc.transform.Find("sweater2").Find("pCylinder1").GetComponent<SkinnedMeshRenderer>().material.color = PlayerColor.Get(i);
				// Setup the progress indicator
				npc.GetComponent<ProgressIndicator>().Initialize(manager.prefabSource.Find("SpotlightProgress").gameObject);
			}
		}

		// Spawn NPCs
		// Spawn people
		for (int i = 0; i < manager.personCount; i++) {
			// Spawn person
			GameObject npc = SpawnNPC(sourcePerson, GetRandomPointOnMap(sourcePerson.transform.position.y), Quaternion.identity, npcs);
			// Set skin color
			npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[0].color = SkinColor.GetRandom();
			npc.transform.Find("pCube1").GetComponent<SkinnedMeshRenderer>().materials[1].color = PantColor.GetRandom();
			// Pick a random shirt
			Transform clothing = (Instantiate(shirtSource.GetChild(Random.Range(0, shirtSource.childCount)).gameObject, npc.transform) as GameObject).transform;
			clothing.localPosition = new Vector3(0, 0, 0);
			// Add the Wearable script
			clothing.Find("pCylinder1").gameObject.AddComponent<Wearable>();
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
				Transform accessory = (Instantiate(accessorySource.gameObject, temporary) as GameObject).transform;
				accessory.localPosition = accessorySource.localPosition;
				accessory.rotation = accessorySource.rotation;
				accessory.localScale = accessorySource.localScale;
			}
		}

		// Spawn items
		for (int i = 0; i < manager.itemCount; i++) {
			// Pick a random item
			GameObject source = itemSource.GetChild(Random.Range(0, itemSource.childCount)).gameObject;
			// Spawn it
			SpawnItem(source);
		}

		// Play menu music
		manager.menuSource.Stop();
		// Play ambient sounds
		foreach (AudioSource source in manager.ambientSources) {
			source.Play();
		}

		roundInProgress = true;
		roundIsPaused = false;
		ScoreManager.SetupScores(playerCount);
		manager.overlay.Find("RoundPanel").gameObject.SetActive(true);
	}

	public static void PauseRound() {
		roundIsPaused = true;

		foreach (NPCController controller in controllers) {
			// Pause NPCs
			controller.Pause();
		}

		Transform pauseMenu = manager.overlay.Find("PauseMenu");
		pauseMenu.gameObject.SetActive(true);
		MenuController.SetTopLevelMenu(pauseMenu);
		//MenuController.AddReturnHandler((menu) => {
		//	ResumeRound();
		//});
	}

	public static void ResumeRound() {
		roundIsPaused = false;

		foreach (NPCController controller in controllers) {
			// Resume NPCs
			controller.Resume();
		}
		foreach (KeyValuePair<Player, Transform> pair in playerMap) {
			// Disable spotlights
			pair.Value.Find("Spotlight").Find("SpotlightCollider").GetComponent<MeshCollider>().enabled = true;
		}

		Transform pauseMenu = manager.overlay.Find("PauseMenu");
		manager.overlay.Find("PauseMenu").gameObject.SetActive(false);
		MenuController.Clear();
	}

	public static void EndRound() {
		// Disable player controls
		ControlManager.UnregisterControls(typeof(SpotlightControl));

		// Stop all NPCs
		foreach (Transform child in npcs) {
			StopNPC(child);
		}
		foreach (KeyValuePair<Player, Transform> pair in playerMap) {
			StopNPC(pair.Value.Find("Ronaldo"));
		}

		// Update agents one final time
		foreach (NPCController controller in controllers) {
			controller.UpdateAgent();
		}

		// Make all ronaldos turn and wave
		foreach (KeyValuePair<Player, Transform> pair in playerMap) {
			pair.Value.Find("Ronaldo").GetComponent<NPCController>().Wave();
		}

		ScoreManager.DisplayResults();

		manager.overlay.Find("RoundEndControls").gameObject.SetActive(true);
		MenuController.SetTopLevelMenu(manager.overlay.Find("RoundEndControls"));
	}

	public static void ResetRound() {
		// Disable player controls
		ControlManager.UnregisterControls(typeof(SpotlightControl));

		playerMap.Clear();
		controllers.Clear();
		spotlightColliders.Clear();
		captorOf.Clear();

		foreach (Transform child in players) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in npcs) {
			Destroy(child.gameObject);
		}
		foreach (Transform child in items) {
			Destroy(child.gameObject);
		}

		roundInProgress = false;
		roundIsPaused = false;

		// Stop ambient sounds
		foreach (AudioSource source in manager.ambientSources) {
			source.Stop();
		}
		// Play menu music
		manager.menuSource.Play();

		manager.overlay.Find("RoundPanel").gameObject.SetActive(false);
		manager.overlay.Find("RoundEndControls").gameObject.SetActive(false);

		MenuController.Clear();
	}

	public static void CaptureRonaldo(GameObject ronaldo, Player captor) {
		Player captive = ronaldo.GetComponent<NPCTraits>().player;

		// Keep track of which player captured which player's ronaldo
		captorOf.Add(captive, captor);

		// Turn off Ronaldo when found
		ronaldo.SetActive(false);
		ronaldo.transform.position = GetRandomPointOnMap(ronaldo.transform.position.y, new Vector3(manager.spawnDistanceFromSpotlight, 10, manager.spawnDistanceFromSpotlight));
		// Add points to the corresponding spotlight's score over time
		ScoreManager.StartIncreasingScoreBy(captor, captive, 1);
		ScoreManager.Jail(captive, captor);

		// Spawn Key after a delay
		manager.StartCoroutine(spawnKey(5.0f));
	}

	public static void ReleaseRonaldo(Player player) {
		Transform playerGroup = GetPlayerGroup(player);

		// Stop increasing the captor's score
		ScoreManager.StopIncreasingScoreBy(captorOf[player], player);
		ScoreManager.Release(player);
		captorOf.Remove(player);
		// Enable the released ronaldo
		playerGroup.Find("Ronaldo").gameObject.SetActive(true);
	}

	public static bool IsRoundInProgress() {
		return roundInProgress;
	}

	public static bool IsRoundPaused() {
		return roundIsPaused;
	}

	string TimestampToString(float timestamp) {
		timestamp += 1;
		int minutes = Mathf.FloorToInt(timestamp / 60);
		timestamp -= minutes * 60;
		int seconds = Mathf.CeilToInt(timestamp) - 1;

		return minutes + ":" + (seconds < 10 ? "0" + seconds : "" + seconds);
	}

	public static GameObject SpawnNPC(GameObject original, Vector3 position, Quaternion rotation, Transform parent) {
		GameObject npc = Instantiate(original, position, rotation, parent) as GameObject;
		// Add NPCController
		controllers.Add(npc.AddComponent<NPCController>());
		// Enable the Nav Mesh Agent
		npc.GetComponent<NavMeshAgent>().enabled = true;
		// Enable the Animator
		npc.GetComponent<Animator>().enabled = true;

		return npc;
	}

	public static GameObject SpawnItem(GameObject original) {
		// Get a random position
		Vector3 position = GameManager.GetRandomPointOnMap(original.transform.position.y);
		// Spawn the item
		GameObject item = Instantiate(original, position, original.transform.rotation, items) as GameObject;
		// Tag as an item
		item.tag = "Item";
		// Spawn the animation effect
		Transform sparkle = (Instantiate(manager.itemAnimation, position, Quaternion.identity, item.transform) as GameObject).transform;
		sparkle.localPosition = new Vector3(0, 0.5f, 0.5f);
		// Set the animation size
		sparkle.localScale = new Vector3(4, 4, 4);
		// Setup the progress indicator
		item.GetComponent<ProgressIndicator>().Initialize(manager.prefabSource.Find("SpotlightProgress").gameObject);
		// Enable the item script
		item.GetComponent<ItemEffect>().enabled = true;

		return item;
	}

	public static void StopNPC(Transform npc) {
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
		foreach (Transform spotlight in spotlightColliders) {
			Vector3 spotlightPosition = spotlight.position;
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

		SpawnItem(manager.prefabSource.Find("key").gameObject);
	}
}
