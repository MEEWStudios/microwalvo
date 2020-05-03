using UnityEngine;

public class PlayParticle : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
		this.GetComponent<ParticleSystem>().Play();
	}

	// Update is called once per frame
	void Update() {

	}
}
