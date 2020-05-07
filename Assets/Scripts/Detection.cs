using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detection : MonoBehaviour {
	private delegate void CollisionAction(GameObject gameObject);

	private class Collision {
		public CollisionAction action;
		public GameObject target;
		public float timer;

		public Collision(CollisionAction action, GameObject target, float delay) {
			timer = delay;
			this.target = target;
			this.action = action;
		}

		public void Run(float deltaTime) {
			timer -= deltaTime;

			if (timer <= 0) {
				action(target);
			}
		}
	}

	public GameObject managersObject;
	public GameObject ronaldo;
	public GameObject explosion;
	public GameObject pickup;
	private readonly Dictionary<GameObject, Collision> collisions = new Dictionary<GameObject, Collision>();

	public SpotlightControl control;

	private int spawnRepel;

	public AudioSource audioSource;
	public AudioClip smokeBomb;
	public float smokeBombVolume;

	//Vibration variables
	private readonly float vibrationTimer = 0.5f;

	void Start() {
		control = transform.parent.GetComponent<SpotlightControl>();
		spawnRepel = GameManager.manager.spawnDistanceFromSpotlight;
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject gameObject = collider.gameObject;

		// Check if the Ronaldo found is not their own 
		if (gameObject.CompareTag("Real Ronaldo") && gameObject.GetComponent<NPCTraits>().player != control.player) {
			Debug.Log("Ronaldo entered spotlight!");
			collisions.Add(gameObject, new Collision(MoveRonaldo, gameObject, 2f));
		}

		if (gameObject.CompareTag("Fake Ronaldo")) {
			Debug.Log("Fake Ronaldo entered spotlight!");
			collisions.Add(gameObject, new Collision(MoveFakeRonaldo, gameObject, 2f));
		}

		if (gameObject.CompareTag("Item")) {
			ItemEffect effect = gameObject.GetComponent<ItemEffect>();

			if (effect.CanActivate(transform.parent.parent)) {
				collisions.Add(gameObject, new Collision(ActivateItem, gameObject, 1f));
			}
		}
	}

	private void OnTriggerExit(Collider collider) {
		GameObject gameObject = collider.gameObject;

		if (collisions.ContainsKey(gameObject)) {
			collisions.Remove(gameObject);
		}

		if (gameObject.CompareTag("Real Ronaldo") && gameObject.GetComponent<NPCTraits>().player != control.player) {
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
		GameManager.CaptureRonaldo(ronaldo, control.player);

		// Remove the collision
		collisions.Remove(ronaldo);
	}

	void MoveFakeRonaldo(GameObject fakeRonaldo) {
		// Spawn explosion effect
		Instantiate(explosion, fakeRonaldo.transform.position, Quaternion.identity);

		// Play explosion sound effect
		audioSource.PlayOneShot(smokeBomb, smokeBombVolume);
		EZGamepad gamepad = EZGM.GetEZGamepad(control.player);
		if (gamepad != null) {
			gamepad.HapticsOverTime(vibrationTimer, 1, 0);
		}
		// Move lookalike
		fakeRonaldo.transform.position = GameManager.GetRandomPointOnMap(fakeRonaldo.transform.position.y, new Vector3(spawnRepel, 10, spawnRepel));

		// Subtract points from corresponding spotlight
		ScoreManager.ChangeScoreBy(control.player, -20);

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
		effect.Activate(GameManager.GetPlayerGroup(control.player));

		// Remove the collision
		collisions.Remove(item);
	}
}
