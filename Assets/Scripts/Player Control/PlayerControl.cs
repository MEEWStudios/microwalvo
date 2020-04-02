using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControl : MonoBehaviour {
	public virtual void Update() {

	}

	public abstract void ProcessGamepadInput(EZGamepad gamepad);
}
