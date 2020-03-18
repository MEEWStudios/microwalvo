using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTexture : MonoBehaviour {

	public Camera cam;
	public Color drawColor;
	public Renderer destinationRenderer;
	public int TextureSize;
	public int Radius;
	public Color BlurColor;
	public GameObject spotlight;
    public GameObject TopViewCamera;

	private Texture2D texture;



	void Start ()
	{

		texture = new Texture2D(TextureSize, TextureSize, TextureFormat.RFloat, false, true); 
		for (int i = 0; i < texture.height; i++)
		{
			for (int j = 0; j < texture.width; j++)
			{
				texture.SetPixel(i, j, BlurColor);
			}
		}
		texture.Apply();
		destinationRenderer.material.SetTexture("_MouseMap", texture);
	}

	void OnSpotlight ()
	{


			Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);


			int x = (int)spotlight.transform.position.x;
			int y = (int)spotlight.transform.position.z;

			texture.SetPixel(x, y, color);

			for (int i = 0; i < texture.height; i++)
			{
				for (int j = 0; j < texture.width; j++)
				{
					float dist = Vector2.Distance(new Vector2(i,j), new Vector2(x,y));
					if(dist <= Radius)
						texture.SetPixel(i, j, color);
				}
			}

			texture.Apply();
			destinationRenderer.material.SetTexture("_MouseMap", texture);
        
	}

	void OnMouseDrag ()
	{
	 Ray ray = cam.ScreenPointToRay(spotlight.transform.position); //Input.mousePosition
		
Debug.Log(ray);
Debug.Log(spotlight.transform.position);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 100))
        {
        	Debug.Log("RAYCAST HIT");
			// younger = redder (higher r)
			// older = blacker
			//Debug.Log("Time: " + Time.LevelLoad + "; r: " + r);
			Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);
			//Debug.Log("r: " + color.r);
			//Color color = new Color(1, 0, 0, 1);

			int x = (int)(hit.textureCoord.x*texture.width);
			int y = (int)(hit.textureCoord.y*texture.height);

			texture.SetPixel(x, y, color);

			for (int i = 0; i < texture.height; i++)
			{
				for (int j = 0; j < texture.width; j++)
				{
					float dist = Vector2.Distance(new Vector2(i,j), new Vector2(x,y));
					if(dist <= Radius)
						texture.SetPixel(i, j, color);
				}
			}

			texture.Apply();
			destinationRenderer.material.SetTexture("_MouseMap", texture);
        }
	}

	void Update()
    {
                Debug.DrawRay(spotlight.transform.position , (cam.transform.position - spotlight.transform.position), Color.yellow);
                
    }


}