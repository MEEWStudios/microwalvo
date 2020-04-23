using UnityEngine;

public static class PlayerColor {
	private static readonly Color[] PLAYER_COLORS = new Color[] { new Color(244 / 255f, 67 / 255f, 54 / 255f), new Color(176 / 255f, 66 / 255f, 245 / 255f), new Color(72 / 255f, 142 / 255f, 240 / 255f), new Color(58 / 255f, 218 / 255f, 100 / 255f) };

	public static Color Get(Player player) {
		return PLAYER_COLORS[(int) player];
	}

	public static Color Get(int player) {
		return PLAYER_COLORS[player];
	}
}
