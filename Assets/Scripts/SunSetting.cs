using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSetting : MonoBehaviour {

    public float rotationDegrees;
    public float timeToTurn;

    float rotationStep;
    float timer = 0;

    Light lt;

	// Use this for initialization
	void Start () {
        rotationStep = rotationDegrees / timeToTurn;
        lt = GetComponent<Light>();
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
        }
	}


    //Input a number of degrees to rotate, input a time to rotate

    //divide degrees by seconds to rotate 

    //rotate every second by that many degrees
}
