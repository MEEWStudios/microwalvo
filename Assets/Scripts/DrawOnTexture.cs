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
	public GameObject spotlight1;
	public GameObject spotlight2;

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

	// Debugging 
	void OnMouseDrag ()
	{
		//Debug.Log("MOUSE POSITION: " + Input.mousePosition); //(x: 0 to 6800, y: 0 to 2304, z: 0)
		//Debug.Log("SPOTLIGHT POSITION: " + spotlight.transform.position); //(position: x: -112 to 124, z: -34 to 63)
        //Debug.Log(cam.WorldToScreenPoint(spotlight.transform.position).x);
	}




	void Update()
    {

    	// create a vector that uses x and y coordinates that were converted from world to screen coordinates
        Vector3 spotlight1Vector = new Vector3(cam.WorldToScreenPoint(spotlight1.transform.position).x, cam.WorldToScreenPoint(spotlight1.transform.position).y, 0.0f);

        Vector3 spotlight2Vector = new Vector3(cam.WorldToScreenPoint(spotlight2.transform.position).x, cam.WorldToScreenPoint(spotlight2.transform.position).y, 0.0f);

        //the ray for the spotlight
        Ray ray = cam.ScreenPointToRay(spotlight1Vector); //Input.mousePosition 
        Ray ray2 = cam.ScreenPointToRay(spotlight2Vector); 


        RaycastHit hit;
        RaycastHit hit2;

    	//spotlight1
        if(Physics.Raycast(ray, out hit, 100))
        {
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








        //spotlight2
        if(Physics.Raycast(ray2, out hit2, 100))
        {
			// younger = redder (higher r)
			// older = blacker
			//Debug.Log("Time: " + Time.LevelLoad + "; r: " + r);
			Color color = new Color(Time.timeSinceLevelLoad, 0, 0, 1);
			//Debug.Log("r: " + color.r);
			//Color color = new Color(1, 0, 0, 1);

			int x = (int)(hit2.textureCoord.x*texture.width);
			int y = (int)(hit2.textureCoord.y*texture.height);

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


}