using UnityEngine;
using System.Collections;

public static class SkinColor {
	public static readonly int[,] COLORS = new int[,] { { 231, 192, 164 }, { 241, 194, 125 }, { 224, 172, 105 }, { 198, 134, 66 }, { 141, 85, 36 } };
	//public static readonly int[,] COLORS = new int[,] { { 231, 192, 164 }, { 229, 177, 133 }, { 241, 194, 125 }, { 224, 172, 105 }, { 198, 134, 66 }, { 141, 85, 36 } };
	//public static readonly Color[] COLORS = new Color[] { new Color(231 / 255f, 192 / 255f, 164 / 255f), new Color(229 / 255f, 177 / 255f, 133 / 255f), new Color(241 / 255f, 194 / 255f, 125 / 255f), new Color(224 / 255f, 172 / 255f, 105 / 255f), new Color(198 / 255f, 134 / 255f, 66 / 255f), new Color(141 / 255f, 85 / 255f, 36 / 255f) };

	public static Color GetRandom() {
		int color = Random.Range(0, COLORS.GetLength(0));
		//return new Color(COLORS[color, 0], COLORS[color, 1], COLORS[color, 2]);
		float r = COLORS[color, 0] / 255f;
		return new Color(r, COLORS[color, 1] / 255f, COLORS[color, 2] / 255f);
	}
}
