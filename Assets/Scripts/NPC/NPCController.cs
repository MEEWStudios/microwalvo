using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {
	public float wanderRadius = 100;
	public float minIdleTime = 0;
	public float maxIdleTime = 15;
	public float panicTime = 5;
	public float maxStuckTime = 2;

	private NavMeshAgent agent;
	private Animator animator;

	private bool hasReachedDestination = false;
	// panicTimer counts down to zero
	private float panicTimer = 0;
	// stuckTimer counts up
	private float stuckTimer = 0;
	// idleTimer counts down to zero
	private float idleTimer = 0;
	private float defaultSpeed;
	private float defaultAcceleration;
	private float defaultAngularSpeed;
	private NavMeshPath lastPath;
	private Vector3 lastDestination;
	private Vector3 lastVelocity;

	void OnEnable() {
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<Animator>();
		defaultSpeed = agent.speed;
		defaultAcceleration = agent.acceleration;
		defaultAngularSpeed = agent.angularSpeed;
		// Reset panic
		panicTimer = 0;

		hasReachedDestination = true;
		idleTimer = Random.Range(minIdleTime, 3);
	}

	// Reset values before the ronaldo is disabled
	public void OnDisable() {
		agent.speed = defaultSpeed;
		agent.acceleration = defaultAcceleration;
		agent.angularSpeed = defaultAngularSpeed;
	}

	// UpdateAgent is called once per frame while the agent is being updated
	public void UpdateAgent() {
		if (agent.enabled) {
			UpdateNavigation();
			UpdateAnimationState();
		}
	}

	// https://answers.unity.com/questions/351049/navmesh-agent-pause.html
	public void Pause() {
		lastPath = agent.path;
		lastDestination = agent.destination;
		lastVelocity = agent.velocity;

		GetComponent<NavMeshAgent>().enabled = false;
		GetComponent<Animator>().enabled = false;
	}

	public void Resume() {
		if (!gameObject.activeSelf) {
			return;
		}

		GetComponent<NavMeshAgent>().enabled = true;
		GetComponent<Animator>().enabled = true;

		if (lastPath != null) {
			agent.SetPath(lastPath);
		} else if (lastDestination.Equals(Vector3.positiveInfinity)) {
			agent.SetDestination(lastDestination);
		}

		agent.velocity = lastVelocity;
		lastPath = null;
		lastDestination = Vector3.positiveInfinity;
	}

	public void Wave() {
		if (!gameObject.activeSelf) {
			return;
		}

		animator.SetInteger("moveState", 4);
		animator.speed = 3;
		agent.enabled = true;
		agent.updatePosition = false;
		agent.SetDestination(transform.position - new Vector3(0, 0, 5));
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

	private void UpdateNavigation() {
		CheckIfPanic();
		CheckIfStuck();

		// https://answers.unity.com/questions/324589/how-can-i-tell-when-a-navmesh-has-reached-its-dest.html
		// Check if the destination has been reached
		if (!hasReachedDestination && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
			if (!agent.hasPath || agent.velocity.sqrMagnitude < 0f) {
				OnReachDestination();
			}
		}

		if (hasReachedDestination) {
			idleTimer -= Time.deltaTime;

			if (idleTimer <= 0) {
				hasReachedDestination = false;
				// Set new destination
				agent.SetDestination(RandomNavSphere(transform.position, wanderRadius));
			}
		}
	}

	private void UpdateAnimationState() {
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
	}

	private void CheckIfPanic() {
		// Check if panicking
		if (panicTimer > 0) {
			panicTimer -= Time.deltaTime;
			// Increase speed
			agent.speed = defaultSpeed * 2.5f;
			agent.acceleration = defaultAcceleration * 2.5f;
			agent.angularSpeed = defaultAngularSpeed * 2.5f;
			// Prevent pausing between destinations
			idleTimer = 0;
		} else {
			agent.speed = defaultSpeed;
			agent.acceleration = defaultAcceleration;
			agent.angularSpeed = defaultAngularSpeed;
		}
	}

	private void CheckIfStuck() {
		// Check if stuck
		if (!hasReachedDestination) {
			// Change destination if stuck for more than maxStuckTime
			// If panicking, change destination after half a second
			if (stuckTimer > (panicTimer > 0 ? 0.5f : maxStuckTime)) {
				stuckTimer = 0;
				// Set new destination
				agent.SetDestination(RandomNavSphere(transform.position, wanderRadius));
			}
			if (agent.velocity.sqrMagnitude < 1) {
				stuckTimer += Time.deltaTime;
			}
		}
	}

	private void OnReachDestination() {
		hasReachedDestination = true;
		idleTimer = Random.Range(minIdleTime, maxIdleTime);
	}

	private static Vector3 RandomNavSphere(Vector3 origin, float dist) {
		Vector3 randomPoint;
		NavMeshHit hit;
		do {
			randomPoint = (Random.insideUnitSphere * dist) + origin;
			if (Vector3.Distance(origin, randomPoint) < 1f) {
			}
		} while (!NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas));

		return hit.position;
	}
}
