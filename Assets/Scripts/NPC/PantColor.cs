using UnityEngine;

public static class PantColor {
	public static readonly int[,] COLORS = new int[,] { { 20, 20, 34 }, { 255, 255, 255 }, { 138, 138, 138 }, { 161, 112, 16 }, { 42, 65, 128 }, { 128, 42, 42 } };

	public static Color GetRandom() {
		int color = Random.Range(0, COLORS.GetLength(0));

		return new Color(COLORS[color, 0] / 255f, COLORS[color, 1] / 255f, COLORS[color, 2] / 255f);
	}
}
