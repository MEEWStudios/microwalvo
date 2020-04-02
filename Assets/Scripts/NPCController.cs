using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NPCController : MonoBehaviour {

	public float wanderRadius = 60; // Wander Radius
	public float minWanderTime = 3;
	public float maxWanderTime = 8;

	private float wanderTimer;
	private Transform target;
	private NavMeshAgent agent;
	private float timer;

	// Use this for initialization
	void OnEnable() {
		agent = GetComponent<NavMeshAgent>();
		timer = wanderTimer;
	}

	// Update is called once per frame
	void Update() {
		timer += Time.deltaTime;

		if (timer >= wanderTimer) {
			Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
			agent.SetDestination(newPos);
			timer = 0;
			wanderTimer = Random.Range(minWanderTime, maxWanderTime);
		}
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask) {
		Vector3 randDirection = Random.insideUnitSphere * dist;

		randDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

		return navHit.position;
	}
}
