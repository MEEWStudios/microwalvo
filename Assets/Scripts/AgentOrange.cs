using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentOrange : MonoBehaviour {

	[SerializeField]
	public Transform destination;
	private NavMeshAgent agent;


	// Start is called before the first frame update
	void Start()
    {
		agent = GetComponent<NavMeshAgent>();

		if (agent == null) {
			Debug.Log("HI");
		}
		agent.SetDestination(destination.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
