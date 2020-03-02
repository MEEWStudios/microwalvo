using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detect_thing : MonoBehaviour
{

    public float light_x_position = 0;
    public float light_z_position = 0;
    public float ronaldo_x_position = 0;
    public float ronaldo_z_position = 0;
    public float fake_ronaldo_x_position = 0;
    public float fake_ronaldo_z_position = 0;
    public float good_item_x_position = 0;
    public float good_item_z_position = 0;
    public float bad_item_x_position = 0;
    public float bad_item_z_position = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    bool ronaldoWithinSpotlight() 
    {
        if(ronaldo_x_position >= (light_x_position - 3) && ronaldo_x_position <= (light_x_position + 3) && ronaldo_z_position >= (light_z_position - 3) && ronaldo_z_position <= (light_z_position + 3)) {
            return true;
        }
        else {
            return false;
        }
    }

      bool fakeRonaldoWithinSpotlight() 
    {
        if(fake_ronaldo_x_position >= (light_x_position - 3) && fake_ronaldo_x_position <= (light_x_position + 3) && fake_ronaldo_z_position >= (light_z_position - 3) && fake_ronaldo_z_position <= (light_z_position + 3)) {
            return true;
        }
        else {
            return false;
        }
    }

      bool goodItemWithinSpotlight() 
    {
        if(good_item_x_position >= (light_x_position - 3) && good_item_x_position <= (light_x_position + 3) && good_item_z_position >= (light_z_position - 3) && good_item_z_position <= (light_z_position + 3)) {
            return true;
        }
        else {
            return false;
        }
    }

      bool badItemWithinSpotlight() 
    {
        if(bad_item_x_position >= (light_x_position - 3) && bad_item_x_position <= (light_x_position + 3) && bad_item_z_position >= (light_z_position - 3) && bad_item_z_position <= (light_z_position + 3)) {
            return true;
        }
        else {
            return false;
        }
    }

    bool detectableThingWithinSpotlight()
    {
       if(ronaldoWithinSpotlight() || fakeRonaldoWithinSpotlight() || goodItemWithinSpotlight() || badItemWithinSpotlight())
       {
           return true;
       }
       else
       {
           return false; 
       }
    }


    // Update is called once per frame
    void Update()
    {
        //Constantly get position of the spotlight and detectable objects
        light_x_position = GameObject.Find("Spotlight").transform.position.x;
        light_z_position = GameObject.Find("Spotlight").transform.position.z;

        ronaldo_x_position = GameObject.Find("Ronaldo").transform.position.x;
        ronaldo_z_position = GameObject.Find("Ronaldo").transform.position.z;

        fake_ronaldo_x_position = GameObject.Find("Fake Ronaldo").transform.position.x;
        fake_ronaldo_z_position = GameObject.Find("Fake Ronaldo").transform.position.z;

        good_item_x_position = GameObject.Find("Good Item").transform.position.x;
        good_item_z_position = GameObject.Find("Good Item").transform.position.z;

        bad_item_x_position = GameObject.Find("Bad Item").transform.position.x;
        bad_item_z_position = GameObject.Find("Bad Item").transform.position.z;


    /*
        if spotlight within radius of ronaldo, fake ronaldo, or any item
        start a 3 second countdown
        if countdown reaches 0 and still within radius, use the four bool functions
        to see what is under the spotlight and report it back
     */

      if(detectableThingWithinSpotlight())
      {
          Debug.Log("Detectable Object Found!");

          if(ronaldoWithinSpotlight())
          {
              Debug.Log("Ronaldo Found!");
          }
          if(fakeRonaldoWithinSpotlight())
          {
              Debug.Log("Fake Ronaldo Found!");
          }
          if(goodItemWithinSpotlight())
          {
              Debug.Log("Good Item Found!");
          }
          if(badItemWithinSpotlight())
          {
              Debug.Log("Bad Item Found!");
          }
      } 
    
        
    }
}
