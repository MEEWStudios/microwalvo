using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {
	private Dictionary<Team, int> scores = new Dictionary<Team, int>();

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	void ResetScores() {
		scores.Clear();
	}
}
