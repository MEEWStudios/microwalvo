using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detection : MonoBehaviour {
	private delegate void CollisionAction(GameObject gameObject);

	private class Collision {
		public CollisionAction action;
		public GameObject target;
		public ItemEffect effect;
		public float delay;
		public float timer;
		private readonly Detection detection;

		public Collision(CollisionAction action, GameObject target, float delay, Detection detection) {
			this.action = action;
			this.target = target;
			this.delay = delay;
			timer = delay;
			effect = target.GetComponent<ItemEffect>();
			this.detection = detection;
		}

		public void Run(float deltaTime) {
			timer -= deltaTime;

			if (effect != null) {
				effect.SetProgress(detection, 1 - (timer / delay));
			}

			if (timer <= 0) {
				action(target);
			}
		}

		public void Cancel() {
			if (effect != null) {
				effect.CancelProgress(detection);
			}
		}
	}

	[Header("General")]
	public GameObject managersObject;
	public GameObject ronaldo;
	public GameObject explosion;
	public GameObject pickup;
	[Header("Audio")]
	public AudioSource audioSource;
	public AudioClip smokeBomb;
	public float smokeBombVolume;
	[HideInInspector]
	public Player player;

	private int spawnRepel;
	private readonly Dictionary<GameObject, Collision> collisions = new Dictionary<GameObject, Collision>();
	//Vibration variables
	private readonly float vibrationTimer = 0.5f;

	void Start() {
		spawnRepel = GameManager.manager.spawnDistanceFromSpotlight;
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject gameObject = collider.gameObject;

		// Check if the Ronaldo found is not their own 
		if (gameObject.CompareTag("Real Ronaldo") && gameObject.GetComponent<NPCTraits>().player != player) {
			Debug.Log("Ronaldo entered spotlight!");
			collisions.Add(gameObject, new Collision(MoveRonaldo, gameObject, 2f, this));
		}

		if (gameObject.CompareTag("Fake Ronaldo")) {
			Debug.Log("Fake Ronaldo entered spotlight!");
			collisions.Add(gameObject, new Collision(MoveFakeRonaldo, gameObject, 2f, this));
		}

		if (gameObject.CompareTag("Item")) {
			ItemEffect effect = gameObject.GetComponent<ItemEffect>();

			if (effect.CanActivate(transform.parent.parent)) {
				collisions.Add(gameObject, new Collision(ActivateItem, gameObject, 2f, this));
			}
		}
	}

	private void OnTriggerExit(Collider collider) {
		GameObject gameObject = collider.gameObject;

		if (collisions.ContainsKey(gameObject)) {
			collisions[gameObject].Cancel();
			collisions.Remove(gameObject);
		}

		if (gameObject.CompareTag("Real Ronaldo") && gameObject.GetComponent<NPCTraits>().player != player) {
			Debug.Log("Ronaldo exited spotlight!");
		}

		if (gameObject.CompareTag("Fake Ronaldo")) {
			Debug.Log("Fake Ronaldo exited spotlight!");
		}
	}

	public void UpdateCollisions() {
		float deltaTime = Time.deltaTime;

		foreach (Collision collision in collisions.Values.ToList()) {
			collision.Run(deltaTime);
		}
	}

	void MoveRonaldo(GameObject ronaldo) {
		// Spawn explosion effect
		Instantiate(explosion, ronaldo.transform.position, Quaternion.identity);

		// Play explosion sound effect
		audioSource.PlayOneShot(smokeBomb, smokeBombVolume);
		// Capture ronaldo
		GameManager.CaptureRonaldo(ronaldo, player);

		// Remove the collision
		collisions.Remove(ronaldo);
	}

	void MoveFakeRonaldo(GameObject fakeRonaldo) {
		// Spawn explosion effect
		Instantiate(explosion, fakeRonaldo.transform.position, Quaternion.identity);

		// Play explosion sound effect
		audioSource.PlayOneShot(smokeBomb, smokeBombVolume);
		EZGamepad gamepad = EZGM.GetEZGamepad(player);
		if (gamepad != null) {
			gamepad.HapticsOverTime(vibrationTimer, 1, 0);
		}
		// Move lookalike
		fakeRonaldo.transform.position = GameManager.GetRandomPointOnMap(fakeRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));

		// Subtract points from corresponding spotlight
		ScoreManager.ChangeScoreBy(player, -20);

		// Remove the collision
		collisions.Remove(fakeRonaldo);
	}

	void ActivateItem(GameObject item) {
		// Spawn pickup effect
		Instantiate(pickup, item.transform.position, Quaternion.identity);

		// Hide item
		item.transform.position = new Vector3(1000, 1000, 1000);
		// Activate the item
		ItemEffect effect = item.GetComponent<ItemEffect>();
		effect.Activate(GameManager.GetPlayerGroup(player));

		// Remove the collision
		collisions.Remove(item);
	}
}
