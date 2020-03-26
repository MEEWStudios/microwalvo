using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour {

	public Camera cam;
	public Color drawColor;
	public Renderer destinationRenderer;
	public int textureDensity;
	public int radius;
	public Color blurColor;
	public GameObject spotlight1;
	public GameObject spotlight2;

	private int textureWidth;
	private int textureHeight;
	private Texture2D texture;
	private Color[,] textureCache;

	void Start() {
		textureWidth = Mathf.FloorToInt(textureDensity * this.gameObject.transform.localScale.x + 0.5f);
		textureHeight = Mathf.FloorToInt(textureDensity * this.gameObject.transform.localScale.z + 0.5f);

		texture = new Texture2D(textureWidth, textureHeight, TextureFormat.RFloat, false, true);
		textureCache = new Color[textureWidth, textureHeight];
		for (int i = 0; i < textureWidth; i++) {
			for (int j = 0; j < textureHeight; j++) {
				textureCache[i, j] = blurColor;
				texture.SetPixel(i, j, blurColor);
			}
		}
		texture.Apply();
		destinationRenderer.material.SetTexture("_MouseMap", texture);
	}

	// Debugging 
	void OnMouseDrag() {
		//Debug.Log("MOUSE POSITION: " + Input.mousePosition); //(x: 0 to 6800, y: 0 to 2304, z: 0)
		//Debug.Log("SPOTLIGHT POSITION: " + spotlight.transform.position); //(position: x: -112 to 124, z: -34 to 63)
		//Debug.Log(cam.WorldToScreenPoint(spotlight.transform.position).x);
	}

	void Update() {
		// create a vector that uses x and y coordinates that were converted from world to screen coordinates
		Vector3 spotlight1Vector = new Vector3(cam.WorldToScreenPoint(spotlight1.transform.position).x, cam.WorldToScreenPoint(spotlight1.transform.position).y, 0.0f);
		Vector3 spotlight2Vector = new Vector3(cam.WorldToScreenPoint(spotlight2.transform.position).x, cam.WorldToScreenPoint(spotlight2.transform.position).y, 0.0f);

		//the ray for the spotlight
		Ray ray = cam.ScreenPointToRay(spotlight1Vector); //Input.mousePosition 
		Ray ray2 = cam.ScreenPointToRay(spotlight2Vector);

		RaycastHit hit;
		RaycastHit hit2;

		bool dirty = false;

		//if (Physics.Raycast(ray, out hit, 100)) {
		//	Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);
		//	int x = (int) (hit.textureCoord.x * texture.width);
		//	int y = (int) (hit.textureCoord.y * texture.height);

		//	if (textureCache[x, y] != color) {
		//		dirty = true;
		//		texture.SetPixel(x, y, color);
		//	}

		//	DateTime before = DateTime.Now;
		//	List<Vector2> coordinates = GetCoordinatesWithinRadius(new Vector2(x, y), radius);
		//	DateTime after = DateTime.Now;
		//	Debug.Log(coordinates.Count);
		//	Debug.Log(after - before);
		//	foreach (Vector2 coordinate in coordinates) {
		//		if (textureCache[(int) coordinate.x, (int) coordinate.y] != color) {
		//			dirty = true;
		//			texture.SetPixel((int) coordinate.x, (int) coordinate.y, color);
		//		}
		//	}
		//}

		//spotlight1
		if (Physics.Raycast(ray, out hit, 100)) {
			// younger = redder (higher r)
			// older = blacker
			//Debug.Log("Time: " + Time.LevelLoad + "; r: " + r);
			Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);
			//Debug.Log("r: " + color.r);
			//Color color = new Color(1, 0, 0, 1);

			int x = (int) (hit.textureCoord.x * texture.width);
			int y = (int) (hit.textureCoord.y * texture.height);

			if (textureCache[x, y] != color) {
				dirty = true;
				texture.SetPixel(x, y, color);
			}

			for (int i = 0; i < textureWidth; i++) {
				for (int j = 0; j < textureHeight; j++) {
					//float dist = Mathf.Sqrt(Mathf.Pow(x - i, 2) + Mathf.Pow(y - j, 2));
					float dist = Vector2.Distance(new Vector2(i, j), new Vector2(x, y));
					if (dist <= radius) {
						if (textureCache[i, j] != color) {
							dirty = true;
							texture.SetPixel(i, j, color);
							//break;
						}
					}
				}
			}
		}

		//spotlight2
		if (Physics.Raycast(ray2, out hit2, 100)) {
			// younger = redder (higher r)
			// older = blacker
			//Debug.Log("Time: " + Time.LevelLoad + "; r: " + r);
			Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);
			//Debug.Log("r: " + color.r);
			//Color color = new Color(1, 0, 0, 1);

			int x = (int) (hit2.textureCoord.x * texture.width);
			int y = (int) (hit2.textureCoord.y * texture.height);

			if (textureCache[x, y] != color) {
				dirty = true;
				texture.SetPixel(x, y, color);
			}

			for (int i = 0; i < textureWidth; i++) {
				for (int j = 0; j < textureHeight; j++) {
					//float dist = Mathf.Sqrt(Mathf.Pow(x - i, 2) + Mathf.Pow(y - j, 2));
					float dist = Vector2.Distance(new Vector2(i, j), new Vector2(x, y));
					if (dist <= radius) {
						if (textureCache[i, j] != color) {
							dirty = true;
							texture.SetPixel(i, j, color);
							//break;
						}
					}
				}
			}
		}

		if (dirty) {
			texture.Apply();
			destinationRenderer.material.SetTexture("_MouseMap", texture);
		}

	}



	List<Vector2> GetCoordinatesWithinRadius(Vector2 origin, int radius) {
		Stack<Vector2> open = new Stack<Vector2>();
		open.Push(origin);
		List<Vector2> closed = new List<Vector2>();
		List<Vector2> result = new List<Vector2>();

		while (open.Count > 0) {
			Vector2 current = open.Pop();

			if (closed.Contains(current)) {
				continue;
			} else {
				closed.Add(current);
			}

			float distance = Vector2.Distance(current, origin);

			if (distance < radius + 0.5) {
				result.Add(current);

				open.Push(new Vector2(current.x, current.y - 1));
				open.Push(new Vector2(current.x + 1, current.y));
				open.Push(new Vector2(current.x, current.y + 1));
				open.Push(new Vector2(current.x - 1, current.y));
			}
		}

		return result;
	}
}
