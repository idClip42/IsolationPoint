using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject[] players;        //list of players -> access positions and alive status
    public int activePlayers;           //score at the end
    public GameObject[] locations;      //locations the enemy can wander to -> nodes for AI
                                        //2nd layer that holds nodes within a location to wander between?

    public float daylightLeft;
    bool night;

	// Use this for initialization
	void Start () {
        InitializeVariables();
	}

    void InitializeVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers = players.Length;
        night = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!night)
        {
            daylightLeft -= Time.deltaTime;
            if (daylightLeft < 0)
            {
                night = true;
            }
        }
	}
}
