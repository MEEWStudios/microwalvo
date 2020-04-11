using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {

	public float wanderRadius = 30; // Wander Radius
	public float minIdleTime = 0;
	public float maxIdleTime = 8;
	public bool debug = false;

	private Transform target;
	private NavMeshAgent agent;
	private Animator animator;
	private float idleTime;
	private float timer = 0;
	private bool hasReachedDestination = false;

	// Use this for initialization
	void OnEnable() {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
		if (debug) {
			Debug.Log(agent.velocity.sqrMagnitude);
		}

		// Set animation state
		if (agent.velocity.sqrMagnitude < 0.5f) {
			animator.SetInteger("moveState", 0);
			animator.speed = 1;
		} else {
			animator.SetInteger("moveState", 1);
			animator.speed = Mathf.Max(0.5f, agent.velocity.sqrMagnitude / 27);
		}

		// Check if stuck
		if (!hasReachedDestination) {
			// If stuck for more than 2 seconds, change destination
			if (timer > 2) {
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

	public static Vector3 RandomNavSphere(Vector3 origin, float dist) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = (Random.insideUnitSphere * dist) + origin;
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return hit.position;
	}
}
