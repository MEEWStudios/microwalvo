using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class SAGEResolutionFix : MonoBehaviour {

	[DllImport("user32.dll")]
	static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
	[DllImport("user32.dll")]
	static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	static extern IntPtr GetForegroundWindow();

	private int cyberResX = 1360 * 5;
	private int cyberResY = 768 * 3;

	// Use this for initialization
	void Awake() {
		if (!Application.isEditor) {
			StartCoroutine("delayedPositionSet");
		}
	}

	IEnumerator delayedPositionSet() {
		yield return new WaitForSeconds(1);
		uint SWP_SHOWWINDOW = 0x0040;
		int GWL_STYLE = -16;
		int WS_BORDER = 1;

		SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_BORDER);
		SetWindowPos(GetForegroundWindow(), 0, 0, 0, cyberResX, cyberResY, SWP_SHOWWINDOW);
	}
}
