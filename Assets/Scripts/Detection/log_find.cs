using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class log_find : MonoBehaviour
{

    void Awake()
    {
        if(GameObject.Find("Ronaldo") != null)
        {
            Debug.Log("Ronaldo exists.");
        }
        else 
        {
            Debug.Log("Ronaldo doesn't exist.");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
