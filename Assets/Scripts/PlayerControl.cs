using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControl : MonoBehaviour
{
	public abstract void ProcessGamepadInput(EZGamepad gamepad);
}
