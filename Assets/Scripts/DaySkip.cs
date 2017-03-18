using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaySkip : MonoBehaviour, IInteractable {

    GameObject sun;

	// Use this for initialization
	void Start () {
        sun = GameObject.Find("Sun");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Action()
    {
        sun.GetComponent<SunSetting>().NightFalls();
    }
}
