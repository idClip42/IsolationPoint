using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject[] players;        //list of players -> access positions and alive status
    public int activePlayers;           //score at the end
    public GameObject[] locations;      //lists of locations the enemy can wander to -> nodes for AI
                                        //2nd layer that holds nodes within a location to wander between

    public float daylightLeft;
    bool night;
    public bool gameover;

	// Use this for initialization
	void Start () {
        Random.InitState((int)Time.time);
        InitializeVariables();
	}

    void InitializeVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers = players.Length;
        night = false;
        gameover = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (gameover)
        {
            //end game screen
            return;
        }

        if (!night)
        {
            daylightLeft -= Time.deltaTime;
            if (daylightLeft < 0)
            {
                night = true;
            }
        }
	}

    /// <summary>
    /// Get the bool night value.
    /// </summary>
    /// <returns>True when night has fallen.</returns>
    public bool GetNight()
    {
        return night;
    }
}
