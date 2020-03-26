using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscMenu : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		//if (Keyboard.current.escapeKey.isPressed) {
		//	this.gameObject.SetActive(!this.gameObject.activeSelf);
		//}
		if (Input.GetKey(KeyCode.Escape)) {
			//this.gameObject.SetActive(this.gameObject.activeSelf);
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#else
			Application.Quit();
#endif
		}
	}
}
