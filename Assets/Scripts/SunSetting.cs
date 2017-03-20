using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSetting : MonoBehaviour {

    public float rotationDegrees;
    public float timeToTurn;

    float rotationStep;
    float timer = 0;
    bool night = false;
    public bool Night { get { return night; } }

    Light lt;

	// Use this for initialization
	void Start () {
        rotationStep = rotationDegrees / timeToTurn;
        lt = GetComponent<Light>();
		ResetSun();
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(timer <= timeToTurn)
        {
            transform.Rotate(new Vector3(rotationStep*Time.deltaTime,0,0));
            timer += Time.deltaTime;
            if(timer>= timeToTurn-4)
            {
                lt.intensity = lt.intensity - 0.01f;
            }
        }
        else
        {
            lt.intensity = 0;
            night = true;
        }
	}


    //Input a number of degrees to rotate, input a time to rotate

    //divide degrees by seconds to rotate 

    //rotate every second by that many degrees



	/// <summary>
	/// Moves the sun into the proper daylight position and state
	/// This way, it can be off and in the night rotation so that no daylight is baked into the lighting
	/// </summary>
	void ResetSun()
	{
		lt.enabled = true;
		transform.Rotate(new Vector3(-rotationDegrees,0,0));
	}


    /// <summary>
    /// Speed up night fall
    /// </summary>
    public void NightFalls()
    {
        float timeLeft = timeToTurn - timer;
        //check that it is still day
        if (timeLeft >= 0)
        {
            //reduce time
            timer /= 5;
            timeToTurn /= 5;
            //speed up rotation
            rotationStep = rotationDegrees / timeToTurn;
        }
    }
}
