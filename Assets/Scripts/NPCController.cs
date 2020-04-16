using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class NPCController : MonoBehaviour {

	public float wanderRadius = 30; // Wander Radius
	public float minIdleTime = 0;
	public float maxIdleTime = 8;
	public float panicTime = 5;
	public bool debug = false;

	private NavMeshAgent agent;
	private Animator animator;
	private float idleTime;
	private float timer = 0;
	private bool hasReachedDestination = false;
	private float panicTimer = 0;
	private float defaultSpeed;
	private float defaultAcceleration;
	private float defaultAngularSpeed;

	// Use this for initialization
	void OnEnable() {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		defaultSpeed = agent.speed;
		defaultAcceleration = agent.acceleration;
		defaultAngularSpeed = agent.angularSpeed;
	}

	// Update is called once per frame
	void Update() {
		if (debug) {
			Debug.Log(agent.velocity.sqrMagnitude);
		}

		// Set animation state
		if (panicTimer > 0) {
			animator.SetInteger("moveState", 3);
			animator.speed = 1;
		} else if (agent.velocity.sqrMagnitude < 0.5f) {
			animator.SetInteger("moveState", 0);
			animator.speed = 1;
		} else if (agent.velocity.sqrMagnitude < 26f) {
			animator.SetInteger("moveState", 1);
			animator.speed = Mathf.Max(0.5f, agent.velocity.sqrMagnitude / 27);
		}

		// Check if panicking
		if (panicTimer > 0) {
			panicTimer -= Time.deltaTime;
			// Increase speed
			agent.speed = defaultSpeed * 2.5f;
			agent.acceleration = defaultAcceleration * 2.5f;
			agent.angularSpeed = defaultAngularSpeed * 2.5f;
			// Prevent pausing between destinations
			idleTime = 0;
		} else {
			agent.speed = defaultSpeed;
			agent.acceleration = defaultAcceleration;
			agent.angularSpeed = defaultAngularSpeed;
		}

		// Check if stuck
		if (!hasReachedDestination) {
			// Change destination if stuck for more than two seconds
			// If panicking, change destination after half a second
			float maxStuckTime = panicTimer > 0 ? 0.5f : 2;
			if (timer > maxStuckTime) {
				timer = 0;
				// Set new destination
				agent.SetDestination(RandomNavSphere(transform.position, wanderRadius));
			}
			if (agent.velocity.sqrMagnitude < 1) {
				timer += Time.deltaTime;
			}
		}

		// https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
		// Check if the destination has been reached
		if (!hasReachedDestination && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
			if (!agent.hasPath || agent.velocity.sqrMagnitude < 0f) {
				hasReachedDestination = true;
				idleTime = Random.Range(minIdleTime, maxIdleTime);
				timer = Time.deltaTime;
			}
		}

		if (hasReachedDestination) {
			timer += Time.deltaTime;

			if (timer >= idleTime) {
				hasReachedDestination = false;
				timer = 0;
				// Set new destination
				agent.SetDestination(RandomNavSphere(transform.position, wanderRadius));
			}
		}
	}

	private void OnTriggerEnter(Collider collider) {
		GameObject gameObject = collider.gameObject;

		if (gameObject.CompareTag("Player")) {
			SpotlightControl control = gameObject.transform.parent.GetComponent<SpotlightControl>();
			NPCTraits traits = GetComponent<NPCTraits>();

			if (traits == null || control.player != traits.player) {
				panicTimer = panicTime;
			}
		}
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float dist) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = (Random.insideUnitSphere * dist) + origin;
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return hit.position;
	}
}
